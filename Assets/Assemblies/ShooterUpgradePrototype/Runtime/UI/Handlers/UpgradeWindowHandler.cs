#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Player.Services;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;
using ShooterUpgradePrototype.UI.UISystem.Loaders;
using ShooterUpgradePrototype.UI.UISystem.Views;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [UIParent(typeof(Root), RootSlots.ScreensRoot)]
    public sealed class UpgradeWindowHandler : UIToolkitUIHandler
    {
        private readonly PlayerStatsService _playerStatsService;
        private readonly IReadOnlyList<string> _upgradeStatIds;
        private readonly Dictionary<string, int> _draftLevels = new();
        private readonly SerialDisposable _showBindings = new();
        private UpgradeWindowView _view;

        public UpgradeWindowHandler(
            DiContainer container,
            UpgradeWindowLayoutLoader loader,
            PlayerStatsService playerStatsService) : base(container, loader)
        {
            _playerStatsService = playerStatsService;
            _upgradeStatIds = _playerStatsService.GetUpgradeWindowStatIds();
        }

        protected override string SpawnedRootName => "ShooterUpgradePrototypeUpgradeWindow";

        protected override async UniTask AfterShowUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _view = GetUIComponent<UpgradeWindowView>(nameof(UpgradeWindowView));
            BindView();

            await CreateStats(cancellationToken);
            _draftLevels.Clear();

            Render();
        }

        protected override UniTask AfterHideUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _showBindings.Disposable = Disposable.Empty;
            return UniTask.CompletedTask;
        }

        private async UniTaskVoid CloseAsync(bool applyChanges)
        {
            if (applyChanges)
            {
                if (_draftLevels.Count > 0 && !_playerStatsService.TryApplyUpgrades(_draftLevels))
                {
                    Render();
                    return;
                }
            }

            _draftLevels.Clear();
            await UINavigator.Hide<UpgradeWindowHandler, Root>(CancellationToken.None);
        }

        private void Render()
        {
            if (_view == null)
            {
                return;
            }

            int availableDraftPoints = GetAvailableDraftPoints();
            _view.SetAvailablePoints(availableDraftPoints);

            for (int i = 0; i < _upgradeStatIds.Count; i++)
            {
                string statId = _upgradeStatIds[i];
                if (!TryGetDynamicChild(statId, out UpgradeStatRowHandler rowHandler))
                {
                    continue;
                }

                RenderRow(statId, rowHandler, availableDraftPoints, i == _upgradeStatIds.Count - 1);
            }

            _view.SetApplyEnabled(_draftLevels.Count > 0);
        }

        private void BindView()
        {
            var showDisposables = new CompositeDisposable();
            _showBindings.Disposable = showDisposables;

            _playerStatsService.AvailableExp
                .Subscribe(_ => Render())
                .AddTo(showDisposables);

            foreach (string statId in _upgradeStatIds)
            {
                _playerStatsService.GetLevelProperty(statId)
                    .Subscribe(_ => Render())
                    .AddTo(showDisposables);
            }

            _view.OnApplyClicked
                .Subscribe(_ => CloseAsync(applyChanges: true).Forget())
                .AddTo(showDisposables);

            _view.OnCloseClicked
                .Subscribe(_ => CloseAsync(applyChanges: false).Forget())
                .AddTo(showDisposables);

            _view.OnBackdropClicked
                .Subscribe(_ => CloseAsync(applyChanges: false).Forget())
                .AddTo(showDisposables);
        }

        private void AddDraftLevel(string statId)
        {
            int availableDraftPoints = GetAvailableDraftPoints();
            int currentDraftLevel = GetDraftLevel(statId);
            int currentLevel = _playerStatsService.GetAppliedLevel(statId);
            int maxLevel = _playerStatsService.GetMaxLevel(statId);

            if (availableDraftPoints <= 0 || currentLevel + currentDraftLevel >= maxLevel)
            {
                return;
            }

            _draftLevels[statId] = currentDraftLevel + 1;
            Render();
        }

        private int GetDraftLevel(string statId) => _draftLevels.TryGetValue(statId, out int draftLevel)
            ? draftLevel
            : 0;

        private int GetAvailableDraftPoints()
        {
            int spentDraftPoints = 0;

            foreach (KeyValuePair<string, int> pair in _draftLevels)
            {
                spentDraftPoints += pair.Value;
            }

            int availablePoints = _playerStatsService.AvailableExp.Value - spentDraftPoints;
            return availablePoints > 0 ? availablePoints : 0;
        }

        private string BuildPendingDeltaText(string statId, int currentLevel, int previewLevel)
        {
            if (previewLevel <= currentLevel)
            {
                return string.Empty;
            }

            float currentValue = _playerStatsService.GetTargetValue(statId, currentLevel);
            float previewValue = _playerStatsService.GetTargetValue(statId, previewLevel);
            float delta = previewValue - currentValue;

            return delta >= 0f ? $"+{delta:0.##}" : delta.ToString("0.##");
        }

        private void RenderRow(string statId, UpgradeStatRowHandler rowHandler, int availableDraftPoints, bool isLast)
        {
            int draftLevels = GetDraftLevel(statId);
            int currentLevel = _playerStatsService.GetAppliedLevel(statId);
            int previewLevel = currentLevel + draftLevels;
            int maxLevel = _playerStatsService.GetMaxLevel(statId);

            rowHandler.SetIsLast(isLast);
            rowHandler.Render(
                previewLevel,
                maxLevel,
                BuildPendingDeltaText(statId, currentLevel, previewLevel),
                draftLevels > 0,
                previewLevel < maxLevel && availableDraftPoints > 0);
        }

        private async UniTask CreateStats(CancellationToken cancellationToken)
        {
            foreach (string statId in _upgradeStatIds)
            {
                bool rowAlreadyExists = TryGetDynamicChild(statId, out UpgradeStatRowHandler _);

                UpgradeStatRowHandler rowHandler =
                    await CreateDynamicChild<UpgradeStatRowHandler>(
                        statId,
                        showAutomatically: true,
                        cancellationToken);

                if (!rowAlreadyExists)
                {
                    rowHandler.UpgradeRequested += HandleUpgradeRequested;
                }
            }
        }

        private void HandleUpgradeRequested(string statId) => AddDraftLevel(statId);

        public override void DisposeUIHandler()
        {
            foreach (string statId in _upgradeStatIds)
            {
                if (TryGetDynamicChild(statId, out UpgradeStatRowHandler rowHandler))
                {
                    rowHandler.UpgradeRequested -= HandleUpgradeRequested;
                }
            }

            _showBindings.Dispose();

            base.DisposeUIHandler();
        }
    }
}
#endif
#endif
#endif
