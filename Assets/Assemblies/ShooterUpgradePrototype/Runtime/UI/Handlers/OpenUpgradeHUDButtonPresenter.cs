#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
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
    [UIParent(typeof(BattleHUDRootPresenter))]
    public sealed class OpenUpgradeHUDButtonPresenter : ParentBoundUIToolkitHandler
    {
        private OpenUpgradeHUDButtonView _view;

        public OpenUpgradeHUDButtonPresenter(DiContainer container)
            : base(container)
        {
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _view = GetView<OpenUpgradeHUDButtonView>(nameof(OpenUpgradeHUDButtonView));

            _view.OnClicked
                .Subscribe(_ => UINavigator.Show<UpgradeWindowPresenter, Root>(cancellationToken).Forget())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
