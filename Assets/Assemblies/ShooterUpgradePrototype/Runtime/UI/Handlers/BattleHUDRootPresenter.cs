#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using UIRootSystem.Runtime;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    [SceneFilter("Battle")]
    [UIParent(typeof(Root), RootSlots.HUD)]
    public sealed class BattleHUDRootPresenter : UIToolkitUIHandler
    {
        public BattleHUDRootPresenter(DiContainer container, BattleHUDLayoutLoader loader)
            : base(container, loader)
        {
        }

        protected override async UniTask InitializeUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);

        protected override string SpawnedRootName => "ShooterUpgradePrototypeBattleHUD";
    }
}
#endif
#endif
#endif
