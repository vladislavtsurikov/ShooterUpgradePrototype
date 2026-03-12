#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Progression.Configs;
using ShooterUpgradePrototype.Progression.Models;
using ShooterUpgradePrototype.Progression.Services;
using ShooterUpgradePrototype.UI.UISystem.Loaders;
using ShooterUpgradePrototype.UI.UISystem.Views;
using UniRx;
using VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [ParentUIHandler(typeof(UIToolkitScreens))]
    public sealed class UpgradeWindowHandler : UIToolkitUIHandler
    {
        private readonly PlayerUpgradeService _playerUpgradeService;
        private readonly UpgradeStatRowLayoutLoader _rowLayoutLoader;

        private PendingUpgradeState _draft;
        private UpgradeWindowView _view;

        public UpgradeWindowHandler(
            DiContainer container,
            UpgradeWindowLayoutLoader loader,
            UpgradeStatRowLayoutLoader rowLayoutLoader,
            PlayerUpgradeService playerUpgradeService) : base(container, loader)
        {
            _rowLayoutLoader = rowLayoutLoader;
            _playerUpgradeService = playerUpgradeService;
        }

        protected override string GetRootName() => "ShooterUpgradePrototypeUpgradeWindow";

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
                BindViewOnce(disposables);
            }

            _draft = _playerUpgradeService.CreateDraft();
            Render();

            return UniTask.CompletedTask;
        }

        private async UniTaskVoid CloseAsync(bool applyChanges)
        {
            if (applyChanges && _draft != null)
            {
                _playerUpgradeService.ApplyDraft(_draft);
            }

            _draft = null;
            await UINavigator.Hide<UpgradeWindowHandler, UIToolkitScreens>(CancellationToken.None);
        }

        private void Render()
        {
            if (_draft == null || _view == null || _rowLayoutLoader.LoadedLayout == null)
            {
                return;
            }

            _view.SetAvailablePointsText($"Available Points: {_draft.AvailablePoints}");

            var rows = _view.EnsureRows(_playerUpgradeService.Tracks.Count, _rowLayoutLoader.LoadedLayout);
            for (int index = 0; index < _playerUpgradeService.Tracks.Count; index++)
            {
                UpgradeTrackConfig track = _playerUpgradeService.Tracks[index];
                BindRow(rows[index], track);
            }

            _view.SetApplyEnabled(_playerUpgradeService.HasPendingChanges(_draft));
        }

        private void BindViewOnce(CompositeDisposable disposables)
        {
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

        private void BindRow(UpgradeStatRowView row, UpgradeTrackConfig track)
        {
            int appliedLevel = _playerUpgradeService.GetAppliedLevel(track.Id);
            int draftLevel = _draft.GetLevel(track.Id);
            int pendingDelta = draftLevel - appliedLevel;
            bool canUpgrade = _draft.AvailablePoints > 0 && draftLevel < track.MaxLevel;

            row.SetTitle(track.DisplayName);
            row.SetPendingDelta(pendingDelta > 0 ? $"+ {pendingDelta}" : string.Empty, pendingDelta > 0);
            row.SetLevel(draftLevel, track.MaxLevel);
            row.SetUpgradeEnabled(canUpgrade);
            row.SetUpgradeRequestedHandler(() =>
            {
                // TODO: move each row to a dedicated presenter after UISystem supports repeated handler instances per row type.
                if (_playerUpgradeService.TryIncrementDraft(_draft, track.Id))
                {
                    Render();
                }
            });
        }
    }
}
#endif
#endif
#endif
