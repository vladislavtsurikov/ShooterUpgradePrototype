#if UI_SYSTEM_UNIRX
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class UnityUIHandler : ChildSpawningUIHandler
    {
        private readonly IUnityUISpawnContextResolver _spawnContextResolver;
        private readonly UnityUIRootController _rootController;

        protected UnityUIHandler(PrefabAssetLoader loader)
            : this(loader, DefaultUnityUISpawnContextResolver.Instance)
        {
        }

        protected UnityUIHandler(
            PrefabAssetLoader loader,
            IUnityUISpawnContextResolver spawnContextResolver)
            : base()
        {
            Loader = loader;
            _rootController = new UnityUIRootController();
            _spawnContextResolver = spawnContextResolver ?? DefaultUnityUISpawnContextResolver.Instance;
        }

        public PrefabAssetLoader Loader { get; }

        public GameObject SpawnedParentPrefab => _rootController.Root;

        public event Action<GameObject, UnityUIHandler> OnAnyChildAdded;

        protected override async UniTask BeforeShowUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => await SpawnMainPrefab(true, cancellationToken);

        protected virtual Transform GetSpawnParentTransform()
        {
            if (Parent == null)
            {
                return null;
            }

            if (Parent is UnityUIHandler unityUIUnit)
            {
                return unityUIUnit.SpawnedParentPrefab.transform;
            }

            throw new InvalidOperationException(
                $"Invalid parent type: {Parent.GetType().Name}. Expected UnityUIHandler.");
        }

        protected virtual string GetParentName() => null;

        internal Transform ResolveSpawnParentTransform() => GetSpawnParentTransform();

        internal string ResolveSpawnParentName() => GetParentName();

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

        protected override async UniTask DestroyChildSpawningUIHandler(bool unload, CancellationToken cancellationToken)
        {
            await _rootController.DestroyAsync(unload, Loader, cancellationToken);
        }

        protected override void DisposeChildSpawningUIHandler() => _rootController.Dispose();

        private async UniTask<GameObject> SpawnMainPrefab(bool enable, CancellationToken cancellationToken)
        {
            if (SpawnedParentPrefab == null)
            {
                (Transform parentTransform, string parentName) = _spawnContextResolver.Resolve(this);

                GameObject spawnedRoot = await _rootController.EnsureSpawnedAsync(
                    Loader,
                    parentTransform,
                    parentName,
                    enable,
                    ComponentBinder,
                    cancellationToken);

                if (Parent != null)
                {
                    var parentUnityUIHandler = (UnityUIHandler)Parent;
                    parentUnityUIHandler.OnAnyChildAdded?.Invoke(spawnedRoot, this);
                }
            }

            return SpawnedParentPrefab;
        }
    }
}
#endif
