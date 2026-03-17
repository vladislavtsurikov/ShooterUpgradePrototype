#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_ZENJECT
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitUIHandler : ChildSpawningUIToolkitHandler
    {
        protected UIToolkitUIHandler(DiContainer container, UIToolkitLayoutLoader loader) : base(container) =>
            Loader = loader;

        public UIToolkitLayoutLoader Loader { get; }
        public VisualElement SpawnedRoot { get; private set; }

        public event Action<VisualElement, UIToolkitUIHandler> OnAnyChildAdded;

        protected virtual string ParentContainerName => null;

        protected virtual string SpawnedRootName => null;

        protected virtual void DisposeUIToolkitUIHandler()
        {
        }

        protected override async UniTask BeforeShowUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await SpawnLayoutIfNeeded(cancellationToken);

        protected override UniTask OnShowUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (SpawnedRoot != null)
            {
                SpawnedRoot.style.display = StyleKeyword.Null;
            }

            return UniTask.CompletedTask;
        }

        protected override UniTask OnHideUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (SpawnedRoot != null)
            {
                SpawnedRoot.style.display = DisplayStyle.None;
            }

            return UniTask.CompletedTask;
        }

        protected override async UniTask DestroyChildSpawningUIHandler(bool unload, CancellationToken cancellationToken)
        {
            if (SpawnedRoot != null)
            {
                SpawnedRoot.RemoveFromHierarchy();
                SpawnedRoot = null;
            }

            if (unload)
            {
                await Loader.Unload(cancellationToken);
            }
        }

        protected override void DisposeChildSpawningUIHandler()
        {
            SpawnedRoot = null;
            DisposeUIToolkitUIHandler();
        }

        private async UniTask SpawnLayoutIfNeeded(CancellationToken cancellationToken)
        {
            if (SpawnedRoot != null)
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

            SpawnedRoot = await this.Spawn()
                .WithParent(parent)
                .Visible(true)
                .WithName(SpawnedRootName)
                .Execute(Loader, ElementBinder, cancellationToken);

            if (SpawnedRoot == null)
            {
                Debug.LogError(
                    $"[UIToolkitUIHandler] Failed to spawn root layout for handler `{GetType().Name}`.");
                return;
            }

            StretchToParent(SpawnedRoot);

            if (Parent is UIToolkitUIHandler parentHandler)
            {
                parentHandler.OnAnyChildAdded?.Invoke(SpawnedRoot, this);
            }
        }

        private VisualElement ResolveParentElement()
        {
            if (Parent == null)
            {
                return GetTopLevelRoot();
            }

            if (Parent is not UIToolkitUIHandler parentHandler)
            {
                throw new InvalidOperationException(
                    $"Invalid parent type: {Parent.GetType().Name}. Expected {nameof(UIToolkitUIHandler)}.");
            }

            string parentContainerName = ParentContainerName;
            if (string.IsNullOrEmpty(parentContainerName))
            {
                parentContainerName = GetParentContainerBindingId();
            }

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

        private string GetParentContainerBindingId()
        {
            var attribute = (UIParentAttribute)Attribute.GetCustomAttribute(
                GetType(),
                typeof(UIParentAttribute));

            return attribute?.ContainerId;
        }

        private VisualElement GetTopLevelRoot()
        {
            if (_container.TryResolve(typeof(UIDocument)) is UIDocument document)
            {
                return document.rootVisualElement;
            }

            UIDocument fallbackDocument = UnityEngine.Object.FindFirstObjectByType<UIDocument>();
            return fallbackDocument?.rootVisualElement;
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
#endif
