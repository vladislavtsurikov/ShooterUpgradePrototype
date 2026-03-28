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

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    [SceneFilter("Battle")]
    [UIParent(typeof(BattleHUDRootPresenter))]
    public sealed class OpenUpgradeHUDButtonPresenter : UIToolkitUIPresenter
    {
        private OpenUpgradeHUDButtonView _view;

        protected override UniTask InitializeUIPresenter(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            _view = ViewResolver.GetView<OpenUpgradeHUDButtonView>(nameof(OpenUpgradeHUDButtonView));

            _view.OnClicked
                .Subscribe(_ => UINavigator.Show<UpgradeWindowPresenter>(cancellationToken).Forget())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
