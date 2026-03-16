#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Player.Services;
using ShooterUpgradePrototype.UI.UISystem.Loaders;
using ShooterUpgradePrototype.UI.UISystem.Views;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [DynamicUIChild]
    public sealed class UpgradeStatRowHandler : UIToolkitUIHandler
    {
        private readonly PlayerStatsService _playerStatsService;
        private UpgradeStatRowView _view;
        private bool _showPendingDelta;
        private bool _upgradeEnabled;
        private int _maxLevel;
        private int _previewLevel;
        private string _pendingDeltaText = string.Empty;
        private string _title = string.Empty;

        public event Action<string> UpgradeRequested;

        protected override string ParentContainerName => "rowsContainer";
        protected override string SpawnedRootName => $"ShooterUpgradePrototypeUpgradeStatRow:{InstanceKey}";

        public UpgradeStatRowHandler(
            DiContainer container,
            UpgradeStatRowLayoutLoader loader,
            PlayerStatsService playerStatsService) : base(container, loader)
        {
            _playerStatsService = playerStatsService;
        }

        protected override UniTask AfterShowUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_view == null)
            {
                _view = GetUIComponent<UpgradeStatRowView>(nameof(UpgradeStatRowView));
            }

            BindView(disposables);

            ApplyViewState();
            return UniTask.CompletedTask;
        }

        public void Render(int previewLevel, int maxLevel, string pendingDeltaText, bool showPendingDelta,
            bool upgradeEnabled)
        {
            _previewLevel = previewLevel;
            _maxLevel = maxLevel;
            _pendingDeltaText = pendingDeltaText;
            _showPendingDelta = showPendingDelta;
            _upgradeEnabled = upgradeEnabled;

            ApplyViewState();
        }

        public void SetIsLast(bool isLast)
        {
            _view?.SetIsLast(isLast);
        }

        private void BindView(CompositeDisposable disposables)
        {
            _view.OnUpgradeClicked
                .Subscribe(_ => UpgradeRequested?.Invoke(InstanceKey))
                .AddTo(disposables);

            _title = _playerStatsService.GetLocalizedStatName(InstanceKey);
        }

        private void ApplyViewState()
        {
            if (_view == null)
            {
                return;
            }

            _view.SetTitle(_title);
            _view.SetLevel(_previewLevel, _maxLevel);
            _view.SetPendingDelta(_pendingDeltaText, _showPendingDelta);
            _view.SetUpgradeEnabled(_upgradeEnabled);
        }
    }
}
#endif
#endif
#endif
