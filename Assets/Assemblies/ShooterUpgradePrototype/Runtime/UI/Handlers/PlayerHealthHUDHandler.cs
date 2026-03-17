#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Player.Services;
using ShooterUpgradePrototype.UI.UISystem.Views;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [UIParent(typeof(BattleHUDRootHandler))]
    public sealed class PlayerHealthHUDHandler : ParentBoundUIToolkitHandler
    {
        private const string HealthStatId = "HP";

        private readonly PlayerStatsService _playerStatsService;
        private PlayerHealthHUDView _view;

        public PlayerHealthHUDHandler(DiContainer container, PlayerStatsService playerStatsService) : base(container)
        {
            _playerStatsService = playerStatsService;
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _view = GetUIComponent<PlayerHealthHUDView>(nameof(PlayerHealthHUDView));

            _playerStatsService.GetValueProperty(HealthStatId)
                .Subscribe(_ => RenderHealth())
                .AddTo(disposables);

            _playerStatsService.GetLevelProperty(HealthStatId)
                .Subscribe(_ => RenderHealth())
                .AddTo(disposables);

            RenderHealth();

            return UniTask.CompletedTask;
        }

        private void RenderHealth()
        {
            float currentValue = _playerStatsService.GetCurrentValue(HealthStatId);
            float maxValue = _playerStatsService.GetCurrentMaxValue(HealthStatId);

            _view.SetHealthText(currentValue, maxValue);
        }
    }
}
#endif
#endif
