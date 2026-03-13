#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.UI.UISystem.Loaders;
using ShooterUpgradePrototype.UI.UISystem.Views;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
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
        private readonly UpgradeStatRowLayoutLoader _rowLayoutLoader;
        private UpgradeWindowView _view;

        public UpgradeWindowHandler(
            DiContainer container,
            UpgradeWindowLayoutLoader loader,
            UpgradeStatRowLayoutLoader rowLayoutLoader) : base(container, loader)
        {
            _rowLayoutLoader = rowLayoutLoader;
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

            Render();

            return UniTask.CompletedTask;
        }

        private async UniTaskVoid CloseAsync(bool applyChanges)
        {
            // TODO: apply draft changes after Progression service is restored.
            await UINavigator.Hide<UpgradeWindowHandler, UIToolkitScreens>(CancellationToken.None);
        }

        private void Render()
        {
            if (_view == null)
            {
                return;
            }

            // TODO: render real upgrade tracks after Progression configs/models/services are restored.
            _view.SetAvailablePointsText("Available Points: --");
            _view.EnsureRows(0, _rowLayoutLoader.LoadedLayout);
            _view.SetApplyEnabled(false);
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
    }
}
#endif
#endif
#endif
