#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using AutoStrike.Services;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace AutoStrike.UI.UISystemIntegration
{
    [SceneFilter("AutoStrikeLegacyBattle")]
    [ParentUIHandler(typeof(UIToolkitHUD))]
    public sealed class AutoStrikeBattleHUDHandler : UIToolkitUIHandler
    {
        private readonly KillCounterService _killCounterService;
        private Label _killsLabel;
        private bool _isBound;

        public AutoStrikeBattleHUDHandler(
            DiContainer container,
            AutoStrikeBattleHUDLayoutLoader loader,
            KillCounterService killCounterService) : base(container, loader) =>
            _killCounterService = killCounterService;

        protected override async UniTask InitializeUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables) => await Show(cancellationToken);

        protected override string GetRootName() => "AutoStrikeBattleHUD";

        protected override UniTask AfterShowUIHandler(CancellationToken ct, CompositeDisposable disposables)
        {
            _killsLabel ??= GetUIComponent<Label>("enemiesKilledLabel");

            if (_isBound)
            {
                return UniTask.CompletedTask;
            }

            _isBound = true;

            _killCounterService.Kills
                .DistinctUntilChanged()
                .Subscribe(ApplyKills)
                .AddTo(disposables);

            ApplyKills(_killCounterService.Kills.Value);

            return UniTask.CompletedTask;
        }

        private void ApplyKills(int kills)
        {
            if (_killsLabel != null)
            {
                _killsLabel.text = $"Enemies Killed: {kills}";
            }
        }
    }
}
#endif
#endif
#endif
