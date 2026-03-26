#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class ChildSpawningUIHandler : ComponentBindingUIHandler
    {
        private readonly UnityUISpawnedChildRegistry _spawnedChildren;

        protected ChildSpawningUIHandler()
        {
            _spawnedChildren = new UnityUISpawnedChildRegistry();
        }

        protected async UniTask<GameObject> SpawnChildPrefab(PrefabAssetLoader prefabLoader, Transform parent,
            bool enable, CancellationToken cancellationToken)
        {
            GameObject instance = await UnityCanvasSpawnOperation.Spawn()
                .WithParent(parent)
                .Enable(enable)
                .Execute(prefabLoader, ComponentBinder, cancellationToken);

            _spawnedChildren.Register(instance, prefabLoader);

            return instance;
        }

        protected override async UniTask DestroyUIHandler(bool unload, CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            await _spawnedChildren.DestroyAsync(unload, cancellationToken);

            await DestroyChildSpawningUIHandler(unload, cancellationToken);
        }

        protected virtual UniTask DestroyChildSpawningUIHandler(bool unload, CancellationToken cancellationToken) =>
            UniTask.CompletedTask;

        protected virtual void DisposeChildSpawningUIHandler()
        {
        }

        protected override void DisposeComponentBindingUIHandler()
        {
            _spawnedChildren.Dispose();

            DisposeChildSpawningUIHandler();
        }
    }
}
#endif
