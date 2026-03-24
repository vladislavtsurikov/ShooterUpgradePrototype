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
        private readonly IUIToolkitParentElementResolver _parentElementResolver;
        private readonly UIToolkitRootController _rootController;
        private readonly UIToolkitSpawnedChildRegistry _spawnedChildren;

        protected UIToolkitUIHandler(UIToolkitLayoutLoader loader)
            : this(
                loader,
                SelfUIToolkitBindingContextResolver.Instance,
                DefaultUIToolkitParentElementResolver.Instance)
        {
        }

        protected UIToolkitUIHandler(IUIToolkitBindingContextResolver bindingContextResolver)
            : this(null, bindingContextResolver, DefaultUIToolkitParentElementResolver.Instance)
        {
        }

        protected UIToolkitUIHandler(
            UIToolkitLayoutLoader loader,
            IUIToolkitBindingContextResolver bindingContextResolver)
            : this(loader, bindingContextResolver, DefaultUIToolkitParentElementResolver.Instance)
        {
        }

        protected UIToolkitUIHandler(
            UIToolkitLayoutLoader loader,
            IUIToolkitParentElementResolver parentElementResolver)
            : this(loader, SelfUIToolkitBindingContextResolver.Instance, parentElementResolver)
        {
        }

        protected UIToolkitUIHandler(
            UIToolkitLayoutLoader loader,
            IUIToolkitBindingContextResolver bindingContextResolver,
            IUIToolkitParentElementResolver parentElementResolver)
        {
            Loader = loader;
            _bindingAccess = new UIToolkitBindingAccess(this, bindingContextResolver);
            _rootController = new UIToolkitRootController();
            _spawnedChildren = new UIToolkitSpawnedChildRegistry();
            _parentElementResolver = parentElementResolver ?? DefaultUIToolkitParentElementResolver.Instance;
        }

        public UIToolkitLayoutLoader Loader { get; }
        public VisualElement SpawnedRoot => _rootController.Root;

        public event Action<VisualElement, UIToolkitUIHandler> OnAnyChildAdded;

        protected virtual string ParentContainerName => null;

        protected virtual string SpawnedRootName => null;

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
            VisualElement instance = await this.Spawn()
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

            VisualElement parent = _parentElementResolver.Resolve(this);
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
