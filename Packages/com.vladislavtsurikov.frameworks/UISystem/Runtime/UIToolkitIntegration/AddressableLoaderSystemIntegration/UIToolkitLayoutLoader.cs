#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
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

            try
            {
                LoadedLayout = await LoadAndTrack<VisualTreeAsset>(LayoutAddress, cancellationToken);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"[UIToolkitLayoutLoader] Addressable load failed for `{LayoutAddress}` in `{GetType().Name}`. Falling back to Resources. Exception: {exception.Message}");
                LoadedLayout = TryLoadFromResources();
            }

            _layoutLoaded = LoadedLayout != null;

            return LoadedLayout;
        }

        protected override UniTask UnloadResourceLoader(CancellationToken cancellationToken)
        {
            _layoutLoaded = false;
            LoadedLayout = null;
            return UniTask.CompletedTask;
        }

        private VisualTreeAsset TryLoadFromResources()
        {
            VisualTreeAsset layout = Resources.Load<VisualTreeAsset>(LayoutAddress);
            if (layout != null)
            {
                return layout;
            }

            return Resources.Load<VisualTreeAsset>($"UI/{LayoutAddress}");
        }
    }
}
#endif
