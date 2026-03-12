#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.UI.UISystem.Views;
using UniRx;
using VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem.Handlers
{
    [SceneFilter("Battle")]
    [ParentUIHandler(typeof(BattleHUDRootHandler))]
    public sealed class OpenUpgradeHUDButtonHandler : ParentBoundUIToolkitHandler
    {
        private OpenUpgradeHUDButtonView _view;

        public OpenUpgradeHUDButtonHandler(DiContainer container)
            : base(container)
        {
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _view = GetUIComponent<OpenUpgradeHUDButtonView>(nameof(OpenUpgradeHUDButtonView));

            _view.OnClicked
                .Subscribe(_ => ShowUpgradeWindow(cancellationToken).Forget())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }

        private async UniTaskVoid ShowUpgradeWindow(CancellationToken cancellationToken)
        {
            // TODO: replace with a dedicated navigation guard if re-show behavior becomes an issue.
            await UINavigator.Show<UpgradeWindowHandler, UIToolkitScreens>(cancellationToken);
        }
    }
}
#endif
#endif
