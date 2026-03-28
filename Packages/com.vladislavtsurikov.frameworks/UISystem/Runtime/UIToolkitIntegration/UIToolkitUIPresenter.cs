#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.Core.Runtime.DependencyInjection;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitUIPresenter : UIPresenter
    {
        private readonly UIToolkitElementBinder _elementBinder;
        private VisualElement _spawnedRoot;

        protected UIToolkitUIPresenter(UIToolkitLayoutLoader loader)
        {
            Loader = loader;
            _elementBinder = new UIToolkitElementBinder(this);
        }

        protected UIToolkitUIPresenter()
            : this(null)
        {
        }

        public UIToolkitLayoutLoader Loader { get; }
        public VisualElement SpawnedRoot => _spawnedRoot;

        protected virtual string ParentContainerName => null;

        protected virtual string SpawnedRootName => null;

        protected override bool UsesParentBindingContext => Loader == null;

        protected UIToolkitElementBinder ElementBinder => _elementBinder;

        protected virtual void DisposeUIToolkitUIPresenter()
        {
        }

        protected override UniTask EnsurePresenterRoot(CancellationToken cancellationToken) =>
            SpawnLayoutIfNeeded(cancellationToken);

        protected override void ShowPresenterRoot()
        {
            _spawnedRoot.style.display = StyleKeyword.Null;
        }

        protected override void HidePresenterRoot()
        {
            if (_spawnedRoot != null)
            {
                _spawnedRoot.style.display = DisplayStyle.None;
            }
        }

        protected override async UniTask DestroyPresenterRoot(bool unload, CancellationToken cancellationToken)
        {
            if (_spawnedRoot != null)
            {
                _spawnedRoot.RemoveFromHierarchy();
                _spawnedRoot = null;
            }

            if (Loader != null)
            {
                if (unload)
                {
                    await Loader.Unload(cancellationToken);
                }
            }
        }

        protected override async UniTask DestroyUIPresenter(
            bool unload,
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            await DestroyUIToolkitUIPresenter(unload, cancellationToken);
        }

        protected virtual UniTask DestroyUIToolkitUIPresenter(bool unload, CancellationToken cancellationToken) =>
            UniTask.CompletedTask;

        public override void DisposeUIPresenter()
        {
            _elementBinder.Dispose();
            _spawnedRoot = null;
            DisposeUIToolkitUIPresenter();
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
                    $"[UIToolkitUIPresenter] Cannot resolve parent root for presenter `{GetType().Name}`.");
                return;
            }

            VisualElement spawnedRoot = await EnsureSpawnedRoot(parent, cancellationToken);

            if (spawnedRoot == null)
            {
                Debug.LogError(
                    $"[UIToolkitUIPresenter] Failed to spawn root layout for presenter `{GetType().Name}`.");
                return;
            }
        }

        private VisualElement ResolveParentElement()
        {
            if (Parent == null)
            {
                return ResolveTopLevelRoot();
            }

            if (Parent is not UIToolkitUIPresenter parentPresenter)
            {
                throw new InvalidOperationException(
                    $"Invalid parent type: {Parent.GetType().Name}. Expected {nameof(UIToolkitUIPresenter)}.");
            }

            string parentContainerName = ResolveParentContainerName();
            if (string.IsNullOrEmpty(parentContainerName))
            {
                return parentPresenter.SpawnedRoot;
            }

            if (parentPresenter.ViewResolver.TryGetView(parentContainerName, out VisualElement container))
            {
                return container;
            }

            throw new InvalidOperationException(
                $"[UIToolkitUIPresenter] Parent container `{parentContainerName}` was not found in presenter `{parentPresenter.GetType().Name}`.");
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
            DependencyResolver resolver = DependencyResolverProvider.GetResolver();
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
