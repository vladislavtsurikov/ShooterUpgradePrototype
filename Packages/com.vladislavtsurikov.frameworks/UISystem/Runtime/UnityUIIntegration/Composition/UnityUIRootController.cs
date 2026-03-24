#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public sealed class UnityUIRootController
    {
        public GameObject Root { get; private set; }

        public async UniTask<GameObject> EnsureSpawnedAsync(
            PrefabAssetLoader loader,
            Transform parent,
            string name,
            bool enable,
            UIComponentBinder componentBinder,
            CancellationToken cancellationToken)
        {
            if (Root != null)
            {
                return Root;
            }

            Root = await new UISpawnOperation()
                .WithParent(parent)
                .Enable(enable)
                .WithName(name)
                .Execute(loader, componentBinder, cancellationToken);

            return Root;
        }

        public void Show()
        {
            if (Root != null)
            {
                Root.SetActive(true);
            }
        }

        public void Hide()
        {
            if (Root != null)
            {
                Root.SetActive(false);
            }
        }

        public async UniTask DestroyAsync(bool unload, PrefabAssetLoader loader, CancellationToken cancellationToken)
        {
            if (Root != null)
            {
                Object.Destroy(Root);
                Root = null;
            }

            if (unload)
            {
                await loader.Unload(cancellationToken);
            }
        }

        public void Dispose() => Root = null;
    }
}
#endif
