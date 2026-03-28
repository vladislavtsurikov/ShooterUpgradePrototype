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
    public abstract class UnityUIPresenter : UIPresenter
    {
        private readonly UnityUIComponentBinder _componentBinder;
        private GameObject _spawnedRoot;

        protected virtual string SpawnedRootName => null;

        protected override bool UsesParentBindingContext => Loader == null;

        protected UnityUIComponentBinder ComponentBinder => _componentBinder;

        public PrefabAssetLoader Loader { get; }
        public GameObject SpawnedRoot => _spawnedRoot;

        protected UnityUIPresenter(PrefabAssetLoader loader)
        {
            Loader = loader;
            _componentBinder = new UnityUIComponentBinder(this);
        }

        protected UnityUIPresenter()
            : this(null)
        {
        }

        protected virtual void DisposeUnityUIPresenter()
        {
        }

        protected override async UniTask BeforeShowUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables) => await SpawnMainPrefab(true, cancellationToken);

        protected virtual Transform GetSpawnParentTransform()
        {
            if (Parent == null)
            {
                return null;
            }

            if (Parent is UnityUIPresenter unityUIPresenter)
            {
                return unityUIPresenter.SpawnedRoot != null ? unityUIPresenter.SpawnedRoot.transform : null;
            }

            throw new InvalidOperationException(
                $"Invalid parent type: {Parent.GetType().Name}. Expected {nameof(UnityUIPresenter)}.");
        }

        protected override UniTask OnShowUIPresenter(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _spawnedRoot.SetActive(true);

            return UniTask.CompletedTask;
        }

        protected override UniTask OnHideUIPresenter(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_spawnedRoot != null)
            {
                _spawnedRoot.SetActive(false);
            }


            return UniTask.CompletedTask;
        }

        protected override async UniTask DestroyUIPresenter(
            bool unload,
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            if (_spawnedRoot != null)
            {
                Object.Destroy(_spawnedRoot);
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

        public override void DisposeUIPresenter()
        {
            _componentBinder.Dispose();
            _spawnedRoot = null;
            DisposeUnityUIPresenter();
        }

        private async UniTask<GameObject> SpawnMainPrefab(bool enable, CancellationToken cancellationToken)
        {
            if (Loader == null || SpawnedRoot != null)
            {
                return SpawnedRoot;
            }

            Transform parentTransform = GetSpawnParentTransform();

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
