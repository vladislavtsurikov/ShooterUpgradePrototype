#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.UI.UISystem.Views;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [ParentUIHandler(typeof(BattleHUDRootHandler))]
    public sealed class PlayerHealthHUDHandler : ParentBoundUIToolkitHandler
    {
        private PlayerHealthHUDView _view;

        public PlayerHealthHUDHandler(DiContainer container) : base(container)
        {
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _view = GetUIComponent<PlayerHealthHUDView>(nameof(PlayerHealthHUDView));

            // TODO: restore reactive HP binding after Progression/player runtime services are added back.
            _view.SetHealthText("HP: -- / --");

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
