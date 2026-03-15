using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.UISystem.Runtime.Core.Graph;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UIHandlerManager
    {
        private readonly Dictionary<Node, UIHandler> _activeUIHandlers = new();
        private readonly Dictionary<DynamicUIHandlerKey, UIHandler> _dynamicUIHandlers = new();
        private readonly Dictionary<UIHandler, DynamicUIHandlerKey> _dynamicKeysByHandler = new();
        private readonly List<Func<FilterAttribute, bool>> _filters = new();

        internal static UniTask CurrentAddFilterTask { get; private set; }

        protected virtual UIHandler CreateUIHandler(Type type) => (UIHandler)Activator.CreateInstance(type);

        protected virtual void RegisterInContainer(UIHandler handler)
        {
        }

        protected virtual void BeforeRemoveHandler(UIHandler handler)
        {
        }

        public async UniTask AddFilter(Func<FilterAttribute, bool> filter,
            CancellationToken cancellationToken = default)
        {
            var completion = new UniTaskCompletionSource();
            CurrentAddFilterTask = completion.Task;

            try
            {
                _filters.Add(filter);

                NodeTree tree = GetNodeTree();

                if (tree.Roots.Count == 0)
                {
                    Debug.LogError(
                        "[UIHandlerManager] NodeTree contains no roots. Please check NodeTreeAsset or generation logic.");
                    return;
                }

                foreach (Node root in tree.Roots)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await TraverseMissingUIHandler(root, null, cancellationToken);
                }
            }
            finally
            {
                completion.TrySetResult();
            }
        }

        public void RemoveFilter(Func<FilterAttribute, bool> filter)
        {
            _filters.Remove(filter);
            CleanupInactiveHandlers(_filters);
        }

        public void RemoveExceptGlobalHandlers()
        {
            var filtersToKeep = _filters
                .Where(f => f(new GlobalFilterAttribute()))
                .ToList();

            CleanupInactiveHandlers(filtersToKeep);

            _filters.Clear();
            _filters.AddRange(filtersToKeep);
        }

        public async UniTask<THandler> CreateDynamicChild<THandler>(
            UIHandler parent,
            string instanceKey,
            CancellationToken cancellationToken = default)
            where THandler : UIHandler
        {
            await EnsureHandlersReady();

            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            ValidateDynamicChildType(typeof(THandler));

            var key = new DynamicUIHandlerKey(parent, typeof(THandler), instanceKey);
            if (_dynamicUIHandlers.TryGetValue(key, out UIHandler existingHandler))
            {
                return (THandler)existingHandler;
            }

            THandler handler = (THandler)CreateUIHandler(typeof(THandler));
            AttachHandler(handler, parent, instanceKey);

            _dynamicUIHandlers[key] = handler;
            _dynamicKeysByHandler[handler] = key;

            await handler.Initialize(cancellationToken, handler.Disposables);
            return handler;
        }

        public bool TryGetDynamicChild<THandler>(UIHandler parent, string instanceKey, out THandler handler)
            where THandler : UIHandler
        {
            if (parent != null &&
                _dynamicUIHandlers.TryGetValue(
                    new DynamicUIHandlerKey(parent, typeof(THandler), instanceKey),
                    out UIHandler existingHandler))
            {
                handler = (THandler)existingHandler;
                return true;
            }

            handler = null;
            return false;
        }

        public async UniTask DestroyDynamicChild<THandler>(
            UIHandler parent,
            string instanceKey,
            bool unload,
            CancellationToken cancellationToken = default)
            where THandler : UIHandler
        {
            if (parent == null)
            {
                return;
            }

            var key = new DynamicUIHandlerKey(parent, typeof(THandler), instanceKey);
            if (!_dynamicUIHandlers.TryGetValue(key, out UIHandler handler))
            {
                return;
            }

            await handler.Destroy(unload, cancellationToken);
            parent.RemoveUIHandlerChild(handler);
        }

        private void CleanupInactiveHandlers(List<Func<FilterAttribute, bool>> activeFilters)
        {
            var toRemove = new List<Node>();

            foreach (KeyValuePair<Node, UIHandler> pair in _activeUIHandlers)
            {
                Type type = pair.Key.HandlerType;

                if (type == null || !activeFilters.Any(f => type.MatchesAnyFilter(f)))
                {
                    UIHandler handler = pair.Value;
                    handler.Dispose();
                    BeforeRemoveHandler(handler);
                    toRemove.Add(pair.Key);
                }
            }

            foreach (Node node in toRemove)
            {
                _activeUIHandlers.Remove(node);
            }
        }

        private async UniTask TraverseMissingUIHandler(Node node, UIHandler parent, CancellationToken cancellationToken)
        {
            if (_activeUIHandlers.ContainsKey(node))
            {
                return;
            }

            Type type = node.HandlerType;
            if (type == null)
            {
                Debug.LogError(
                    "[UIHandlerManager] Failed to resolve type for node. Node was probably deserialized incorrectly or type is missing.");
                return;
            }

            if (!_filters.Any(f => type.MatchesAnyFilter(f)))
            {
                return;
            }

            UIHandler handler = CreateUIHandler(type);
            _activeUIHandlers[node] = handler;
            AttachHandler(handler, parent);

            if (handler.Parent == null)
            {
                await handler.Initialize(cancellationToken, handler.Disposables);
            }

            foreach (Node child in node.Children)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await TraverseMissingUIHandler(child, handler, cancellationToken);
            }
        }

        private async UniTask EnsureHandlersReady()
        {
            if (CurrentAddFilterTask.Status == UniTaskStatus.Pending)
            {
                await CurrentAddFilterTask;
            }
        }

        private void AttachHandler(UIHandler handler, UIHandler parent, string instanceKey = null)
        {
            if (parent != null)
            {
                parent.AddUIHandlerChild(handler);
                handler.SetParent(parent);
            }

            handler.SetInstanceKey(instanceKey);
            handler.DestroyedCallback = HandleHandlerDestroyed;

            RegisterInContainer(handler);
        }

        private void HandleHandlerDestroyed(UIHandler handler)
        {
            BeforeRemoveHandler(handler);

            if (_dynamicKeysByHandler.TryGetValue(handler, out DynamicUIHandlerKey key))
            {
                _dynamicKeysByHandler.Remove(handler);
                _dynamicUIHandlers.Remove(key);
            }
        }

        private static NodeTree GetNodeTree()
        {
            NodeTree tree = NodeTreeAsset.Instance?.Tree;
            if (tree != null && tree.Roots.Count > 0)
            {
                return tree;
            }

            return BuildNodeTreeAtRuntime();
        }

        private static NodeTree BuildNodeTreeAtRuntime()
        {
            Type[] allTypes = AllTypesDerivedFrom<UIHandler>.Types
                .Where(type => !IsDynamicChildType(type))
                .ToArray();
            var typeToNode = new Dictionary<Type, Node>();

            foreach (Type type in allTypes)
            {
                var node = new Node
                {
                    HandlerType = type,
                    Filters = type
                        .GetCustomAttributes(typeof(FilterAttribute), true)
                        .Cast<FilterAttribute>()
                        .Select(filter => filter.GetType())
                        .ToList()
                };

                typeToNode[type] = node;
            }

            var roots = new List<Node>();

            foreach (Type type in allTypes)
            {
                Node node = typeToNode[type];
                UIParentAttribute parentAttribute = type
                    .GetCustomAttributes(typeof(UIParentAttribute), true)
                    .Cast<UIParentAttribute>()
                    .FirstOrDefault();

                if (parentAttribute != null && typeToNode.TryGetValue(parentAttribute.ParentType, out Node parentNode))
                {
                    parentNode.Children.Add(node);
                    continue;
                }

                if (parentAttribute != null)
                {
                    Debug.LogWarning(
                        $"[UIHandlerManager] Runtime tree generation skipped `{type.FullName}` because parent `{parentAttribute.ParentType?.FullName}` was not found.");
                    continue;
                }

                roots.Add(node);
            }

            return new NodeTree { Roots = roots };
        }

        private static bool IsDynamicChildType(Type type) =>
            Attribute.IsDefined(type, typeof(DynamicUIChildAttribute), inherit: true);

        private static void ValidateDynamicChildType(Type type)
        {
            if (!IsDynamicChildType(type))
            {
                throw new InvalidOperationException(
                    $"Dynamic child handler `{type.FullName}` must be marked with [{nameof(DynamicUIChildAttribute)}].");
            }
        }

        private readonly struct DynamicUIHandlerKey : IEquatable<DynamicUIHandlerKey>
        {
            public DynamicUIHandlerKey(UIHandler parent, Type handlerType, string instanceKey)
            {
                Parent = parent;
                HandlerType = handlerType;
                InstanceKey = instanceKey ?? string.Empty;
            }

            public UIHandler Parent { get; }
            public Type HandlerType { get; }
            public string InstanceKey { get; }

            public bool Equals(DynamicUIHandlerKey other) =>
                ReferenceEquals(Parent, other.Parent) &&
                HandlerType == other.HandlerType &&
                InstanceKey == other.InstanceKey;

            public override bool Equals(object obj) =>
                obj is DynamicUIHandlerKey other && Equals(other);

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Parent != null ? Parent.GetHashCode() : 0;
                    hashCode = (hashCode * 397) ^ (HandlerType != null ? HandlerType.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ InstanceKey.GetHashCode();
                    return hashCode;
                }
            }
        }
    }
}
