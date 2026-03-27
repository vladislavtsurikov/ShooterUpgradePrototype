#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public abstract class AddressableAssetLoader<TAsset> : ResourceLoader
        where TAsset : Object
    {
        private bool _isLoaded;

        protected abstract string AssetAddress { get; }

        public virtual bool LoadOnStartup => true;

        protected TAsset LoadedAsset { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token)
        {
            if (LoadOnStartup)
            {
                await LoadAssetIfNotLoaded(token);
            }
        }

        protected async UniTask<TAsset> LoadAssetIfNotLoaded(CancellationToken cancellationToken)
        {
            if (_isLoaded)
            {
                return LoadedAsset;
            }

            LoadedAsset = await LoadAndTrack<TAsset>(AssetAddress, cancellationToken);
            _isLoaded = LoadedAsset != null;

            return LoadedAsset;
        }

        protected override UniTask UnloadResourceLoader(CancellationToken cancellationToken)
        {
            _isLoaded = false;
            LoadedAsset = null;
            return UniTask.CompletedTask;
        }
    }
}
#endif
