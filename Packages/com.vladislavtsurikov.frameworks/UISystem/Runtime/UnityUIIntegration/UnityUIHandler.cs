#if UI_SYSTEM_UNIRX
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class UnityUIHandler : UIHandler
    {
        private readonly UIComponentBinder _componentBinder;
        private GameObject _spawnedRoot;

        protected UnityUIHandler(PrefabAssetLoader loader)
        {
            Loader = loader;
            _componentBinder = new UIComponentBinder(this);
        }

        protected UnityUIHandler()
            : this(null)
        {
        }

        public PrefabAssetLoader Loader { get; }
        public GameObject SpawnedRoot => _spawnedRoot;

        protected virtual string SpawnedRootName => null;

        protected virtual bool UsesParentBindingContext => Loader == null;

        protected UIComponentBinder ComponentBinder => _componentBinder;

        protected virtual void DisposeUnityUIHandler()
        {
        }

        protected override async UniTask BeforeShowUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables) => await SpawnMainPrefab(true, cancellationToken);

        protected virtual Transform GetSpawnParentTransform()
        {
            if (Parent == null)
            {
                return null;
            }

            if (Parent is UnityUIHandler unityUIHandler)
            {
                return unityUIHandler.SpawnedRoot != null ? unityUIHandler.SpawnedRoot.transform : null;
            }

            throw new InvalidOperationException(
                $"Invalid parent type: {Parent.GetType().Name}. Expected {nameof(UnityUIHandler)}.");
        }

        internal Transform ResolveSpawnParentTransform() => GetSpawnParentTransform();

        internal override (Type handlerType, string instanceKey) ResolveBindingContext()
        {
            if (!UsesParentBindingContext)
            {
                return (GetType(), InstanceKey);
            }

            return (Parent?.GetType() ?? GetType(), Parent?.InstanceKey);
        }

        protected override UniTask OnShowUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_spawnedRoot != null)
            {
                _spawnedRoot.SetActive(true);
            }

            return UniTask.CompletedTask;
        }

        protected override UniTask OnHideUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_spawnedRoot != null)
            {
                _spawnedRoot.SetActive(false);
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
                    Object.Destroy(_spawnedRoot);
                    _spawnedRoot = null;
                }

                if (unload)
                {
                    await Loader.Unload(cancellationToken);
                }
            }

            await DestroyUnityUIHandler(unload, cancellationToken);
        }

        protected virtual UniTask DestroyUnityUIHandler(bool unload, CancellationToken cancellationToken) =>
            UniTask.CompletedTask;

        public override void DisposeUIHandler()
        {
            _componentBinder.Dispose();
            _spawnedRoot = null;
            DisposeUnityUIHandler();
        }

        private async UniTask<GameObject> SpawnMainPrefab(bool enable, CancellationToken cancellationToken)
        {
            if (Loader == null || SpawnedRoot != null)
            {
                return SpawnedRoot;
            }

            Transform parentTransform = ResolveSpawnParentTransform();

            _spawnedRoot = await UnityCanvasSpawnOperation.Spawn()
                .WithParent(parentTransform)
                .Enable(enable)
                .WithName(SpawnedRootName)
                .Execute(Loader, ComponentBinder, cancellationToken);

            return SpawnedRoot;
        }
    }
}
#endif
