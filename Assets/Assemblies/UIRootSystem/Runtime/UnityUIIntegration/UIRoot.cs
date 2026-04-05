#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.PrefabResourceLoaders;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UnityUIIntegration;

namespace VladislavTsurikov.UIRootSystem.Runtime
{
    [SceneFilter("TestScene_1", "TestScene_2")]
    public class UIRoot : UnityUIPresenter
    {
        public UIRoot(UIRootLoader loader)
            : base(loader)
        {
        }

        protected override async UniTask InitializeUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            OnUIPresenterAfterShow += HandlePresenterShown;
            disposables.Add(Disposable.Create(() => OnUIPresenterAfterShow -= HandlePresenterShown));

            await Show(cancellationToken);
            ReorderLayers();
        }

        protected override string SpawnedRootName => "UIRoot";

        private void HandlePresenterShown(UIPresenter presenter)
        {
            if (presenter.Parent != this || presenter is not UILayer)
            {
                return;
            }

            ReorderLayers();
        }

        private void ReorderLayers()
        {
            if (SpawnedRoot == null || ChildrenModule == null)
            {
                return;
            }

            UILayer[] orderedLayers = ChildrenModule.All
                .OfType<UILayer>()
                .Where(IsLayerAttachedToRoot)
                .OrderBy(layer => layer.GetLayerIndex())
                .ThenBy(layer => layer.SpawnedRoot.transform.GetSiblingIndex())
                .ToArray();

            for (var i = 0; i < orderedLayers.Length; i++)
            {
                orderedLayers[i].SpawnedRoot.transform.SetSiblingIndex(i);
            }
        }

        private bool IsLayerAttachedToRoot(UILayer layer)
        {
            if (layer?.SpawnedRoot == null)
            {
                return false;
            }

            if (layer.SpawnedRoot.transform.parent == SpawnedRoot.transform)
            {
                return true;
            }

            Debug.LogWarning(
                $"[UIRoot] Spawned child '{layer.SpawnedRoot.name}' is not under expected parent '{SpawnedRoot.name}'. Check parenting logic.");
            return false;
        }
    }
}

#endif

#endif

#endif
