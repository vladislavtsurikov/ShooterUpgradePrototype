#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Progression.Services;
using ShooterUpgradePrototype.UI.UISystem.Views;
using UniRx;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [ParentUIHandler(typeof(BattleHUDRootHandler))]
    public sealed class PlayerHealthHUDHandler : ParentBoundUIToolkitHandler
    {
        private readonly PlayerUpgradeService _playerUpgradeService;

        private PlayerHealthHUDView _view;

        public PlayerHealthHUDHandler(DiContainer container, PlayerUpgradeService playerUpgradeService)
            : base(container) => _playerUpgradeService = playerUpgradeService;

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _view = GetUIComponent<PlayerHealthHUDView>(nameof(PlayerHealthHUDView));

            _playerUpgradeService.CurrentHealth
                .CombineLatest(_playerUpgradeService.MaxHealth, FormatHealth)
                .Subscribe(_view.SetHealthText)
                .AddTo(disposables);

            _view.SetHealthText(FormatHealth(
                _playerUpgradeService.CurrentHealth.Value,
                _playerUpgradeService.MaxHealth.Value));

            return UniTask.CompletedTask;
        }

        private static string FormatHealth(float currentHealth, float maxHealth)
        {
            int roundedCurrent = UnityEngine.Mathf.RoundToInt(currentHealth);
            int roundedMax = UnityEngine.Mathf.RoundToInt(maxHealth);
            return $"HP: {roundedCurrent} / {roundedMax}";
        }
    }
}
#endif
#endif
