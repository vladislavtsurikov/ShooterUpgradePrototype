#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Player.Services;
using ShooterUpgradePrototype.UI.UISystem.Loaders;
using ShooterUpgradePrototype.UI.UISystem.Views;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [UIParent(typeof(Root), RootSlots.ScreensRoot)]
    public sealed class UpgradeWindowHandler : UIToolkitUIHandler
    {
        private readonly UpgradeStatRowLayoutLoader _rowLayoutLoader;
        private readonly PlayerStatsService _playerStatsService;
        private readonly IReadOnlyList<string> _upgradeStatIds;
        private readonly Dictionary<string, int> _draftLevels = new();
        private readonly Dictionary<string, string> _localizedTitles = new();
        private UpgradeWindowView _view;
        private int _boundRowCount;
        private bool _isViewBound;

        public UpgradeWindowHandler(
            DiContainer container,
            UpgradeWindowLayoutLoader loader,
            UpgradeStatRowLayoutLoader rowLayoutLoader,
            PlayerStatsService playerStatsService) : base(container, loader)
        {
            _rowLayoutLoader = rowLayoutLoader;
            _playerStatsService = playerStatsService;
            _upgradeStatIds = _playerStatsService.GetUpgradeWindowStatIds();
        }

        protected override string SpawnedRootName => "ShooterUpgradePrototypeUpgradeWindow";

        protected override async UniTask BeforeShowUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            await base.BeforeShowUIHandler(cancellationToken, disposables);
            await _rowLayoutLoader.LoadLayoutIfNotLoaded(cancellationToken);
        }

        protected override UniTask AfterShowUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
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

            _draftLevels.Clear();

            Render();

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
            _view.SetAvailablePointsText($"Available Points: {availableDraftPoints}");

            IReadOnlyList<UpgradeStatRowView> rows = _view.EnsureRows(_upgradeStatIds.Count, _rowLayoutLoader.LoadedLayout);
            BindRowsOnce(rows, Disposables);

            for (int i = 0; i < _upgradeStatIds.Count; i++)
            {
                string statId = _upgradeStatIds[i];
                UpgradeStatRowView row = rows[i];
                int draftLevels = GetDraftLevel(statId);
                int currentLevel = _playerStatsService.GetAppliedLevel(statId);
                int previewLevel = currentLevel + draftLevels;
                int maxLevel = _playerStatsService.GetMaxLevel(statId);

                row.SetTitle(GetDisplayTitle(statId));
                row.SetLevel(previewLevel, maxLevel);
                row.SetPendingDelta(BuildPendingDeltaText(statId, currentLevel, previewLevel), draftLevels > 0);
                row.SetUpgradeEnabled(previewLevel < maxLevel && availableDraftPoints > 0);
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
                BindLocalizedTitle(statId, disposables);

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

        private string GetDisplayTitle(string statId)
        {
            if (_localizedTitles.TryGetValue(statId, out string title) && !string.IsNullOrWhiteSpace(title))
            {
                return title;
            }

            return _playerStatsService.GetStatName(statId);
        }

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

        private void BindRowsOnce(IReadOnlyList<UpgradeStatRowView> rows, CompositeDisposable disposables)
        {
            while (_boundRowCount < rows.Count)
            {
                int rowIndex = _boundRowCount;
                UpgradeStatRowView row = rows[rowIndex];

                row.OnUpgradeClicked
                    .Subscribe(_ => AddDraftLevel(_upgradeStatIds[rowIndex]))
                    .AddTo(disposables);

                _boundRowCount++;
            }
        }

        private void BindLocalizedTitle(string statId, CompositeDisposable disposables)
        {
            string fallbackTitle = _playerStatsService.GetStatName(statId);
            _localizedTitles[statId] = fallbackTitle;

            if (!_playerStatsService.TryGetLocalizedStatName(statId, out LocalizedString localizedString))
            {
                return;
            }

            void HandleStringChanged(string localizedValue)
            {
                _localizedTitles[statId] = string.IsNullOrWhiteSpace(localizedValue)
                    ? fallbackTitle
                    : localizedValue;

                Render();
            }

            localizedString.StringChanged += HandleStringChanged;
            localizedString.RefreshString();

            Disposable.Create(() => localizedString.StringChanged -= HandleStringChanged)
                .AddTo(disposables);
        }
    }
}
#endif
#endif
#endif
