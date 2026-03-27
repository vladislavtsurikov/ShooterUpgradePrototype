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
        private readonly UIToolkitElementBinder _elementBinder;
        private VisualElement _spawnedRoot;

        protected UIToolkitUIHandler(UIToolkitLayoutLoader loader)
        {
            Loader = loader;
            _elementBinder = new UIToolkitElementBinder(this);
        }

        protected UIToolkitUIHandler()
            : this(null)
        {
        }

        public UIToolkitLayoutLoader Loader { get; }
        public VisualElement SpawnedRoot => _spawnedRoot;

        protected virtual string ParentContainerName => null;

        protected virtual string SpawnedRootName => null;

        protected virtual bool UsesParentBindingContext => Loader == null;

        protected UIToolkitElementBinder ElementBinder => _elementBinder;

        protected virtual void DisposeUIToolkitUIHandler()
        {
        }

        protected override async UniTask BeforeShowUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await SpawnLayoutIfNeeded(cancellationToken);

        protected override UniTask OnShowUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_spawnedRoot != null)
            {
                _spawnedRoot.style.display = StyleKeyword.Null;
            }

            return UniTask.CompletedTask;
        }

        protected override UniTask OnHideUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_spawnedRoot != null)
            {
                _spawnedRoot.style.display = DisplayStyle.None;
            }

            return UniTask.CompletedTask;
        }

        protected override async UniTask DestroyUIHandler(
            bool unload,
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            if (Loader != null)
            {
                if (_spawnedRoot != null)
                {
                    _spawnedRoot.RemoveFromHierarchy();
                    _spawnedRoot = null;
                }

                if (unload)
                {
                    await Loader.Unload(cancellationToken);
                }
            }

            await DestroyUIToolkitUIHandler(unload, cancellationToken);
        }

        protected virtual UniTask DestroyUIToolkitUIHandler(bool unload, CancellationToken cancellationToken) =>
            UniTask.CompletedTask;

        public override void DisposeUIHandler()
        {
            _elementBinder.Dispose();
            _spawnedRoot = null;
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

            VisualElement spawnedRoot = await EnsureSpawnedRoot(parent, cancellationToken);

            if (spawnedRoot == null)
            {
                Debug.LogError(
                    $"[UIToolkitUIHandler] Failed to spawn root layout for handler `{GetType().Name}`.");
                return;
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

            if (parentHandler.ViewResolver.TryGetView(parentContainerName, out VisualElement container))
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

        internal override (Type handlerType, string instanceKey) ResolveBindingContext()
        {
            if (!UsesParentBindingContext)
            {
                return (GetType(), InstanceKey);
            }

            return (Parent?.GetType() ?? GetType(), Parent?.InstanceKey);
        }

        internal VisualElement ResolveTopLevelRoot()
        {
            UIDependencyResolver resolver = UIDependencyResolverUtility.GetResolver();
            if (resolver != null &&
                resolver.TryResolve(typeof(UIDocument), out object instance) &&
                instance is UIDocument document)
            {
                return document.rootVisualElement;
            }

            UIDocument fallbackDocument = UnityEngine.Object.FindFirstObjectByType<UIDocument>();
            return fallbackDocument?.rootVisualElement;
        }

        private async UniTask<VisualElement> EnsureSpawnedRoot(
            VisualElement parent,
            CancellationToken cancellationToken)
        {
            if (_spawnedRoot != null)
            {
                return _spawnedRoot;
            }

            _spawnedRoot = await UIToolkitSpawnOperation.Spawn()
                .WithParent(parent)
                .Visible(true)
                .WithName(SpawnedRootName)
                .Execute(Loader, ElementBinder, cancellationToken);

            if (_spawnedRoot != null)
            {
                StretchToParent(_spawnedRoot);
            }

            return _spawnedRoot;
        }

        private static void StretchToParent(VisualElement element)
        {
            element.style.position = Position.Absolute;
            element.style.left = 0;
            element.style.top = 0;
            element.style.right = 0;
            element.style.bottom = 0;
            element.style.width = StyleKeyword.Auto;
            element.style.height = StyleKeyword.Auto;
            element.style.flexGrow = 1;
            element.style.flexShrink = 0;
        }
    }
}
#endif
