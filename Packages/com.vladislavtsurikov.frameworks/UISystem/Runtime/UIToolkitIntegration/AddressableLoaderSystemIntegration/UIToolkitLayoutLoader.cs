#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class UIToolkitLayoutLoader : ResourceLoader
    {
        private bool _layoutLoaded;

        public abstract string LayoutAddress { get; }

        public virtual bool LoadOnStartup => true;

        public VisualTreeAsset LoadedLayout { get; private set; }

        public override async UniTask LoadResourceLoader(CancellationToken token)
        {
            if (LoadOnStartup)
            {
                await LoadLayoutIfNotLoaded(token);
            }
        }

        public async UniTask<VisualTreeAsset> LoadLayoutIfNotLoaded(CancellationToken cancellationToken)
        {
            if (_layoutLoaded)
            {
                return LoadedLayout;
            }

            LoadedLayout = await LoadAndTrack<VisualTreeAsset>(LayoutAddress, cancellationToken);
            _layoutLoaded = true;

            return LoadedLayout;
        }

        protected override UniTask UnloadResourceLoader(CancellationToken cancellationToken)
        {
            _layoutLoaded = false;
            LoadedLayout = null;
            return UniTask.CompletedTask;
        }
    }
}
#endif
