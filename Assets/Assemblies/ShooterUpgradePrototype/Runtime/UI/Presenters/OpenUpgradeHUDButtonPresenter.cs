#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Runtime;
using UIRootSystem.Runtime;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    [SceneFilter("Battle")]
    [UIParent(typeof(BattleHUDRootPresenter))]
    public sealed class OpenUpgradeHUDButtonPresenter : UIToolkitUIHandler
    {
        private OpenUpgradeHUDButtonView _view;

        public OpenUpgradeHUDButtonPresenter()
            : base()
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
