#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Collections.Generic;
using System.Threading;
using AutoStrike.Input.Generated;
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
    [UIParent(typeof(Root), RootSlots.Screens)]
    public sealed class UpgradeWindowPresenter : UIToolkitUIHandler
    {
        private readonly GameplayInputBlocker _gameplayInputBlocker;
        private readonly PlayerStatsService _playerStatsService;
        private readonly IReadOnlyList<string> _upgradeStatIds;

        private readonly Dictionary<string, int> _draftLevels = new();
        private readonly SerialDisposable _showBindings = new();

        private UpgradeWindowView _view;

        public UpgradeWindowPresenter(
            DiContainer container,
            UpgradeWindowLayoutLoader loader,
            PlayerInputActions playerInputActions,
            PlayerStatsService playerStatsService) : base(container, loader)
        {
            _gameplayInputBlocker = new GameplayInputBlocker(playerInputActions);
            _playerStatsService = playerStatsService;
            _upgradeStatIds = _playerStatsService.GetUpgradeWindowStatIds();
        }

        protected override string SpawnedRootName => "ShooterUpgradePrototypeUpgradeWindow";

        protected override async UniTask AfterShowUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _gameplayInputBlocker.DisableGameplayInput();

            _view = GetView<UpgradeWindowView>(nameof(UpgradeWindowView));

            CompositeDisposable showDisposables = new CompositeDisposable();
            _showBindings.Disposable = showDisposables;

            _playerStatsService.AvailableExp
                .Subscribe(_ => Refresh())
                .AddTo(showDisposables);

            foreach (string statId in _upgradeStatIds)
            {
                _playerStatsService.GetLevelProperty(statId)
                    .Subscribe(_ => Refresh())
                    .AddTo(showDisposables);
            }

            _view.OnCloseClicked
                .Subscribe(_ => CloseAsync().Forget())
                .AddTo(showDisposables);

            _view.OnApplyClicked
                .Subscribe(_ => ApplyDraftLevels())
                .AddTo(showDisposables);

            await CreateStats(cancellationToken);

            _draftLevels.Clear();
            Refresh();
        }

        protected override UniTask AfterHideUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _showBindings.Disposable = Disposable.Empty;
            _gameplayInputBlocker.EnableGameplayInput();
            return UniTask.CompletedTask;
        }

        private async UniTaskVoid CloseAsync()
        {
            _draftLevels.Clear();
            await UINavigator.Hide<UpgradeWindowPresenter, Root>(CancellationToken.None);
        }

        private void ApplyDraftLevels()
        {
            if (_draftLevels.Count == 0)
            {
                return;
            }

            Dictionary<string, int> snapshot = new(_draftLevels);

            if (!_playerStatsService.TryApplyUpgrades(snapshot))
            {
                Refresh();
                return;
            }

            _draftLevels.Clear();
            Refresh();
        }

        private void Refresh()
        {
            if (_view == null)
            {
                return;
            }

            int availableDraftPoints = GetAvailableDraftPoints();
            _view.SetAvailablePoints(availableDraftPoints);

            foreach (string statId in _upgradeStatIds)
            {
                if (!TryGetDynamicChild(statId, out UpgradeStatRowPresenter rowHandler))
                {
                    continue;
                }

                int currentLevel = _playerStatsService.GetAppliedLevel(statId);
                int draftLevel = GetDraftLevel(statId);
                int maxLevel = _playerStatsService.GetMaxLevel(statId);

                float currentValue = _playerStatsService.GetCumulativeValue(statId, currentLevel);
                float previewValue = _playerStatsService.GetCumulativeValue(statId, currentLevel + draftLevel);

                rowHandler.UpdateState(
                    statId,
                    currentLevel,
                    draftLevel,
                    maxLevel,
                    availableDraftPoints,
                    currentValue,
                    previewValue);
            }

            _view.SetApplyEnabled(_draftLevels.Count > 0);
        }

        private void AddDraftLevel(string statId)
        {
            int availableDraftPoints = GetAvailableDraftPoints();
            int currentDraftLevel = GetDraftLevel(statId);
            int currentLevel = _playerStatsService.GetAppliedLevel(statId);
            int maxLevel = _playerStatsService.GetMaxLevel(statId);

            if (availableDraftPoints <= 0)
            {
                return;
            }

            if (currentLevel + currentDraftLevel >= maxLevel)
            {
                return;
            }

            _draftLevels[statId] = currentDraftLevel + 1;
            Refresh();
        }

        private int GetDraftLevel(string statId)
        {
            return _draftLevels.TryGetValue(statId, out int value) ? value : 0;
        }

        private int GetAvailableDraftPoints()
        {
            int spent = 0;

            foreach (KeyValuePair<string, int> pair in _draftLevels)
            {
                spent += pair.Value;
            }

            int available = _playerStatsService.AvailableExp.Value - spent;
            return available > 0 ? available : 0;
        }

        private async UniTask CreateStats(CancellationToken cancellationToken)
        {
            foreach (string statId in _upgradeStatIds)
            {
                UpgradeStatRowPresenter row = GetDynamicChild<UpgradeStatRowPresenter>(statId);

                if (row == null)
                {
                    row = await CreateDynamicChild<UpgradeStatRowPresenter>(
                        statId,
                        showAutomatically: true,
                        cancellationToken);

                    row.UpgradeRequested += AddDraftLevel;
                }
            }
        }

        protected override void DisposeUIToolkitUIHandler()
        {
            foreach (string statId in _upgradeStatIds)
            {
                if (TryGetDynamicChild(statId, out UpgradeStatRowPresenter row))
                {
                    row.UpgradeRequested -= AddDraftLevel;
                }
            }

            _showBindings.Dispose();
        }
    }
}
#endif
#endif
#endif
