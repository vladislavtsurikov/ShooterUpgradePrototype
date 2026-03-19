#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    [SceneFilter("Battle")]
    [DynamicUIChild]
    public sealed class UpgradeStatRowPresenter : UIToolkitUIHandler
    {
        private UpgradeStatRowView _view;

        private string _statId;
        private int _currentLevel;
        private int _draftLevel;
        private int _maxLevel;
        private int _availablePoints;
        private float _currentValue;
        private float _previewValue;
        private PlayerStatsService _playerStatsService;

        public event Action<string> UpgradeRequested;

        protected override string ParentContainerName => "rowsContainer";
        protected override string SpawnedRootName => $"ShooterUpgradePrototypeUpgradeStatRow:{InstanceKey}";

        public UpgradeStatRowPresenter(
            DiContainer container,
            UpgradeStatRowLayoutLoader loader,
            PlayerStatsService playerStatsService) : base(container, loader)
        {
            _playerStatsService =  playerStatsService;
        }

        protected override UniTask AfterShowUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            if (_view != null)
            {
                return UniTask.CompletedTask;
            }

            _view = GetView<UpgradeStatRowView>(nameof(UpgradeStatRowView));
            Bind(disposables);

            return UniTask.CompletedTask;
        }

        public void UpdateState(
            string statId,
            int currentLevel,
            int draftLevel,
            int maxLevel,
            int availablePoints,
            float currentValue,
            float previewValue)
        {
            _statId = statId;
            _currentLevel = currentLevel;
            _draftLevel = draftLevel;
            _maxLevel = maxLevel;
            _availablePoints = availablePoints;
            _currentValue = currentValue;
            _previewValue = previewValue;

            Apply();
        }

        private void Bind(CompositeDisposable disposables)
        {
            _view.OnUpgradeClicked
                .Subscribe(_ => UpgradeRequested?.Invoke(InstanceKey))
                .AddTo(disposables);
        }

        private void Apply()
        {
            if (_view == null)
            {
                return;
            }

            int previewLevel = _currentLevel + _draftLevel;
            bool canUpgrade = previewLevel < _maxLevel && _availablePoints > 0;
            bool showPending = _draftLevel > 0;

            float delta = _previewValue - _currentValue;
            string deltaText = showPending
                ? (delta >= 0f ? $"+{delta:0.##}" : delta.ToString("0.##"))
                : string.Empty;

            _view.SetTitle(_playerStatsService.GetLocalizedStatName(_statId));
            _view.SetLevel(previewLevel, _maxLevel);
            _view.SetPendingDelta(deltaText, showPending);
            _view.SetUpgradeEnabled(canUpgrade);

            _view.style.position = Position.Relative;
        }
    }
}
#endif
#endif
#endif
