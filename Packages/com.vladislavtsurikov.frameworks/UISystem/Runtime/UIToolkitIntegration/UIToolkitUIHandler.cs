#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitUIHandler : UIHandler
    {
        private readonly UIToolkitBindingAccess _bindingAccess;
        private readonly UIToolkitRootController _rootController;
        private readonly UIToolkitSpawnedChildRegistry _spawnedChildren;

        protected UIToolkitUIHandler(UIToolkitLayoutLoader loader)
        {
            Loader = loader;
            _bindingAccess = new UIToolkitBindingAccess(this);
            _rootController = new UIToolkitRootController();
            _spawnedChildren = new UIToolkitSpawnedChildRegistry();
        }

        protected UIToolkitUIHandler()
            : this(null)
        {
        }

        public UIToolkitLayoutLoader Loader { get; }
        public VisualElement SpawnedRoot => _rootController.Root;

        public event Action<VisualElement, UIToolkitUIHandler> OnAnyChildAdded;

        protected virtual string ParentContainerName => null;

        protected virtual string SpawnedRootName => null;

        protected virtual bool UsesParentBindingContext => Loader == null;

        protected UIToolkitElementBinder ElementBinder => _bindingAccess.Binder;

        public TElement GetView<TElement>(string bindingId, Type handlerType, int index = 0)
            where TElement : VisualElement => _bindingAccess.GetView<TElement>(bindingId, handlerType, index);

        public virtual TElement GetView<TElement>(string bindingId, int index = 0)
            where TElement : VisualElement => _bindingAccess.GetView<TElement>(bindingId, index);

        public virtual bool TryGetUIComponent<TElement>(string bindingId, out TElement element, int index = 0)
            where TElement : VisualElement => _bindingAccess.TryGetUIComponent(bindingId, out element, index);

        protected virtual void DisposeUIToolkitUIHandler()
        {
        }

        protected async UniTask<VisualElement> SpawnChildLayout(
            UIToolkitLayoutLoader layoutLoader,
            VisualElement parent,
            bool visible,
            CancellationToken cancellationToken,
            string rootName = null)
        {
            VisualElement instance = await UIToolkitSpawnOperation.Spawn()
                .WithParent(parent)
                .Visible(visible)
                .WithName(rootName)
                .Execute(layoutLoader, ElementBinder, cancellationToken);

            _spawnedChildren.Register(instance, layoutLoader);

            return instance;
        }

        protected override async UniTask BeforeShowUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await SpawnLayoutIfNeeded(cancellationToken);

        protected override UniTask OnShowUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _rootController.Show();
            return UniTask.CompletedTask;
        }

        protected override UniTask OnHideUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _rootController.Hide();
            return UniTask.CompletedTask;
        }

        protected override async UniTask DestroyUIHandler(
            bool unload,
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            await _spawnedChildren.DestroyAsync(unload, cancellationToken);

            if (Loader != null)
            {
                await _rootController.DestroyAsync(unload, Loader, cancellationToken);
            }

            await DestroyUIToolkitUIHandler(unload, cancellationToken);
        }

        protected virtual UniTask DestroyUIToolkitUIHandler(bool unload, CancellationToken cancellationToken) =>
            UniTask.CompletedTask;

        public override void DisposeUIHandler()
        {
            _bindingAccess.Dispose();
            _spawnedChildren.Dispose();
            _rootController.Dispose();
            DisposeUIToolkitUIHandler();
        }

        private async UniTask SpawnLayoutIfNeeded(CancellationToken cancellationToken)
        {
            if (Loader == null || SpawnedRoot != null)
            {
                return;
            }

            VisualElement parent = ResolveParentElement();
            if (parent == null)
            {
                Debug.LogError(
                    $"[UIToolkitUIHandler] Cannot resolve parent root for handler `{GetType().Name}`.");
                return;
            }

            VisualElement spawnedRoot = await _rootController.EnsureSpawnedAsync(
                Loader,
                parent,
                SpawnedRootName,
                ElementBinder,
                cancellationToken);

            if (spawnedRoot == null)
            {
                Debug.LogError(
                    $"[UIToolkitUIHandler] Failed to spawn root layout for handler `{GetType().Name}`.");
                return;
            }

            if (Parent is UIToolkitUIHandler parentHandler)
            {
                parentHandler.OnAnyChildAdded?.Invoke(spawnedRoot, this);
            }
        }

        private VisualElement ResolveParentElement()
        {
            if (Parent == null)
            {
                return ResolveTopLevelRoot();
            }

            if (Parent is not UIToolkitUIHandler parentHandler)
            {
                throw new InvalidOperationException(
                    $"Invalid parent type: {Parent.GetType().Name}. Expected {nameof(UIToolkitUIHandler)}.");
            }

            string parentContainerName = ResolveParentContainerName();
            if (string.IsNullOrEmpty(parentContainerName))
            {
                return parentHandler.SpawnedRoot;
            }

            if (parentHandler.TryGetUIComponent(parentContainerName, out VisualElement container))
            {
                return container;
            }

            throw new InvalidOperationException(
                $"[UIToolkitUIHandler] Parent container `{parentContainerName}` was not found in handler `{parentHandler.GetType().Name}`.");
        }

        internal string ResolveParentContainerName()
        {
            string parentContainerName = ParentContainerName;
            if (!string.IsNullOrEmpty(parentContainerName))
            {
                return parentContainerName;
            }

            var attribute = (UIParentAttribute)Attribute.GetCustomAttribute(
                GetType(),
                typeof(UIParentAttribute));

            return attribute?.ContainerId;
        }

        internal (Type handlerType, string instanceKey) ResolveBindingContext()
        {
            if (!UsesParentBindingContext)
            {
                return (GetType(), InstanceKey);
            }

            return (Parent?.GetType() ?? GetType(), Parent?.InstanceKey);
        }

        internal VisualElement ResolveTopLevelRoot()
        {
            if (UIDependencyResolverUtility.TryResolve(typeof(UIDocument), out object instance) &&
                instance is UIDocument document)
            {
                return document.rootVisualElement;
            }

            UIDocument fallbackDocument = UnityEngine.Object.FindFirstObjectByType<UIDocument>();
            return fallbackDocument?.rootVisualElement;
        }
    }
}
#endif
