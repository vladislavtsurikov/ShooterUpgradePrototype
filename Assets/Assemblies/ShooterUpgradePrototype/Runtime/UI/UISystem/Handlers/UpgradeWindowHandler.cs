#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Player.Services;
using UniRx;
using UnityEngine;
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
        private readonly UIHandlerManager _uiHandlerManager;
        private readonly PlayerStatsService _playerStatsService;
        private readonly IReadOnlyList<string> _upgradeStatIds;
        private readonly Dictionary<string, int> _draftLevels = new();
        private readonly Dictionary<string, UpgradeStatRowHandler> _rowHandlers = new();
        private UpgradeWindowView _view;
        private bool _isViewBound;

        public UpgradeWindowHandler(
            DiContainer container,
            UpgradeWindowLayoutLoader loader,
            PlayerStatsService playerStatsService,
            UIHandlerManager uiHandlerManager) : base(container, loader)
        {
            _playerStatsService = playerStatsService;
            _uiHandlerManager = uiHandlerManager;
            _upgradeStatIds = _playerStatsService.GetUpgradeWindowStatIds();
        }

        protected override string SpawnedRootName => "ShooterUpgradePrototypeUpgradeWindow";

        protected override async UniTask AfterShowUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            if (_view == null)
            {
                _view = GetUIComponent<UpgradeWindowView>(nameof(UpgradeWindowView));
            }

            if (!_isViewBound)
            {
                BindViewOnce(disposables);
                _isViewBound = true;
            }

            await EnsureRowHandlers(cancellationToken);
            await ShowRowHandlers(cancellationToken);

            _draftLevels.Clear();

            Render();
        }

        protected override async UniTask OnHideUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            await base.OnHideUIHandler(cancellationToken, disposables);
            await HideRowHandlers(cancellationToken);
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
            _view.SetAvailablePointsText($"Available Points: {availableDraftPoints}");

            for (int i = 0; i < _upgradeStatIds.Count; i++)
            {
                string statId = _upgradeStatIds[i];
                if (!_rowHandlers.TryGetValue(statId, out UpgradeStatRowHandler rowHandler))
                {
                    continue;
                }

                int draftLevels = GetDraftLevel(statId);
                int currentLevel = _playerStatsService.GetAppliedLevel(statId);
                int previewLevel = currentLevel + draftLevels;
                int maxLevel = _playerStatsService.GetMaxLevel(statId);

                rowHandler.Render(
                    previewLevel,
                    maxLevel,
                    BuildPendingDeltaText(statId, currentLevel, previewLevel),
                    draftLevels > 0,
                    previewLevel < maxLevel && availableDraftPoints > 0);
            }

            _view.SetApplyEnabled(_draftLevels.Count > 0);
        }

        private void BindViewOnce(CompositeDisposable disposables)
        {
            _playerStatsService.AvailableExp
                .Subscribe(_ => Render())
                .AddTo(disposables);

            foreach (string statId in _upgradeStatIds)
            {
                _playerStatsService.GetLevelProperty(statId)
                    .Subscribe(_ => Render())
                    .AddTo(disposables);
            }

            _view.OnApplyClicked
                .Subscribe(_ => CloseAsync(applyChanges: true).Forget())
                .AddTo(disposables);

            _view.OnCloseClicked
                .Subscribe(_ => CloseAsync(applyChanges: false).Forget())
                .AddTo(disposables);

            _view.OnBackdropClicked
                .Subscribe(_ => CloseAsync(applyChanges: false).Forget())
                .AddTo(disposables);
        }

        private void AddDraftLevel(string statId)
        {
            int currentDraftLevel = GetDraftLevel(statId);
            int availableDraftPoints = GetAvailableDraftPoints();
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
                spentDraftPoints += Mathf.Max(0, pair.Value);
            }

            return Mathf.Max(0, _playerStatsService.AvailableExp.Value - spentDraftPoints);
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

        private async UniTask EnsureRowHandlers(CancellationToken cancellationToken)
        {
            foreach (string statId in _upgradeStatIds)
            {
                if (_rowHandlers.ContainsKey(statId))
                {
                    continue;
                }

                UpgradeStatRowHandler rowHandler =
                    await _uiHandlerManager.CreateDynamicChild<UpgradeStatRowHandler>(this, statId, cancellationToken);
                rowHandler.UpgradeRequested += HandleUpgradeRequested;
                _rowHandlers[statId] = rowHandler;
            }
        }

        private async UniTask ShowRowHandlers(CancellationToken cancellationToken)
        {
            foreach (UpgradeStatRowHandler rowHandler in _rowHandlers.Values)
            {
                if (rowHandler.IsActive)
                {
                    continue;
                }

                await rowHandler.Show(cancellationToken);
            }
        }

        private async UniTask HideRowHandlers(CancellationToken cancellationToken)
        {
            foreach (UpgradeStatRowHandler rowHandler in _rowHandlers.Values)
            {
                if (!rowHandler.IsActive)
                {
                    continue;
                }

                await rowHandler.Hide(cancellationToken);
            }
        }

        private void HandleUpgradeRequested(string statId) => AddDraftLevel(statId);

        public override void DisposeUIHandler()
        {
            foreach (KeyValuePair<string, UpgradeStatRowHandler> pair in _rowHandlers)
            {
                pair.Value.UpgradeRequested -= HandleUpgradeRequested;
            }

            _rowHandlers.Clear();

            base.DisposeUIHandler();
        }
    }
}
#endif
#endif
#endif
