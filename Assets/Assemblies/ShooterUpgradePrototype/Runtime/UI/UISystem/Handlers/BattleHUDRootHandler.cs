#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.UI.UISystem.Loaders;
using UniRx;
using VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [ParentUIHandler(typeof(UIToolkitHUD))]
    public sealed class BattleHUDRootHandler : UIToolkitUIHandler
    {
        public BattleHUDRootHandler(DiContainer container, BattleHUDLayoutLoader loader)
            : base(container, loader)
        {
        }

        protected override async UniTask InitializeUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);

        protected override string GetRootName() => "ShooterUpgradePrototypeBattleHUD";
    }
}
#endif
#endif
#endif
