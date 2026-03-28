using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.Core.Runtime.DependencyInjection;
using VladislavTsurikov.UISystem.Runtime.Core.Graph;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public class UIHandlerManager
    {
        private readonly Dictionary<Node, UIHandler> _activeUIHandlers = new();
        private readonly List<Func<FilterAttribute, bool>> _filters = new();
        private readonly DependencyResolver _resolver;

        internal static UniTask CurrentAddFilterTask { get; private set; }

        public UIHandlerManager()
        {
            _resolver = DependencyResolverProvider.GetResolver();
        }

        protected virtual UIHandler CreateUIHandler(Type type)
        {
            return _resolver.Instantiate(type) as UIHandler;
        }

        protected virtual void RegisterInContainer(UIHandler handler)
        {
            UIHandlerKey key = UIHandlerKey.FromHandler(handler);
            _resolver.BindInstance(handler.GetType(), key.Id, handler);
        }

        protected virtual void BeforeRemoveHandler(UIHandler handler)
        {
            UIHandlerKey key = UIHandlerKey.FromHandler(handler);
            _resolver.UnbindId(handler.GetType(), key.Id);
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

        internal async UniTask<THandler> CreateDynamicChild<THandler>(
            UIHandler parent,
            string instanceKey,
            bool showAutomatically,
            CancellationToken cancellationToken)
            where THandler : UIHandler
        {
            await UIHandlerResolver.EnsureHandlersReady();

            if (TryGetDynamicChild(parent, instanceKey, out THandler existingHandler))
            {
                if (showAutomatically && !existingHandler.IsActive.Value)
                {
                    await existingHandler.Show(cancellationToken);
                }

                return existingHandler;
            }

            THandler handler = (THandler)CreateUIHandler(typeof(THandler));
            AttachHandler(handler, parent, instanceKey);
            await handler.Initialize(cancellationToken, handler.Disposables);

            if (showAutomatically)
            {
                await handler.Show(cancellationToken);
            }

            return handler;
        }

        internal THandler GetDynamicChild<THandler>(UIHandler parent, string instanceKey)
            where THandler : UIHandler
        {
            TryGetDynamicChild(parent, instanceKey, out THandler handler);
            return handler;
        }

        internal bool TryGetDynamicChild<THandler>(UIHandler parent, string instanceKey, out THandler handler)
            where THandler : UIHandler
        {
            if (parent == null || string.IsNullOrEmpty(instanceKey))
            {
                handler = null;
                return false;
            }

            if (parent.ChildrenModule == null)
            {
                handler = null;
                return false;
            }

            UIHandlerKey key = UIHandlerKey.FromDynamicParent(parent, instanceKey);
            return UIHandlerResolver.TryResolve(key, out handler);
        }

        internal async UniTask DestroyDynamicChild<THandler>(
            UIHandler parent,
            string instanceKey,
            bool unload,
            CancellationToken cancellationToken)
            where THandler : UIHandler
        {
            if (!TryGetDynamicChild(parent, instanceKey, out THandler handler))
            {
                return;
            }

            UIChildrenModule childrenModule = RequireChildrenModule(parent);
            await handler.Destroy(unload, cancellationToken);
            BeforeRemoveHandler(handler);
            childrenModule.Remove(handler);
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

        private void AttachHandler(UIHandler handler, UIHandler parent, string instanceKey = null)
        {
            handler.SetUIHandlerManager(this);

            if (parent != null)
            {
                UIChildrenModule childrenModule = RequireChildrenModule(parent);
                childrenModule.Add(handler);
                handler.SetParent(parent);
            }

            handler.SetInstanceKey(instanceKey);

            RegisterInContainer(handler);
        }

        private static UIChildrenModule RequireChildrenModule(UIHandler handler)
        {
            if (handler?.ChildrenModule != null)
            {
                return handler.ChildrenModule;
            }

            throw new InvalidOperationException(
                $"[UISystem] Handler `{handler?.GetType().FullName}` does not support child handlers.");
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
    }
}
