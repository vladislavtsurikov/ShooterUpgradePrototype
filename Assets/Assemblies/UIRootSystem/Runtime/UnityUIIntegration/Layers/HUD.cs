#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.PrefabResourceLoaders;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UIRootSystem.Runtime.Layers
{
    [UIParent(typeof(UIRoot))]
    [SceneFilter("TestScene_1", "TestScene_2")]
    public class HUD : UILayer
    {
        public HUD(HUDLoader loader)
            : base(loader)
        {
        }

        public override int GetLayerIndex() => 1;

        protected override async UniTask InitializeUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);
    }
}

#endif

#endif

#endif
