#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.UIElements;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class ChildSpawningUIToolkitHandler : ComponentBindingUIToolkitHandler
    {
        private readonly List<(VisualElement instance, UIToolkitLayoutLoader loader)> _spawnedChildren = new();

        protected ChildSpawningUIToolkitHandler(DiContainer container)
            : base(container)
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

            _spawnedChildren.Add((instance, layoutLoader));

            return instance;
        }

        protected override async UniTask DestroyUIHandler(bool unload, CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            if (unload)
            {
                foreach ((VisualElement _, UIToolkitLayoutLoader loader) in _spawnedChildren)
                {
                    await loader.Unload(cancellationToken);
                }
            }

            foreach ((VisualElement instance, UIToolkitLayoutLoader _) in _spawnedChildren)
            {
                instance?.RemoveFromHierarchy();
            }

            _spawnedChildren.Clear();

            await DestroyChildSpawningUIHandler(unload, cancellationToken);
        }

        protected virtual UniTask DestroyChildSpawningUIHandler(bool unload, CancellationToken cancellationToken) =>
            UniTask.CompletedTask;

        protected virtual void DisposeChildSpawningUIHandler()
        {
        }

        protected override void DisposeComponentBindingUIHandler()
        {
            _spawnedChildren.Clear();
            DisposeChildSpawningUIHandler();
        }
    }
}
#endif
