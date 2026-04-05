#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace UIRootSystem.Runtime
{
    [SceneFilter("Battle")]
    [UIParent(typeof(Root), RootSlots.Screens)]
    public sealed class ScreenPresenter : UIToolkitUIPresenter
    {
        private readonly SingleActiveChildPresenterModule _singleActiveChildPresenterModule;

        public ScreenPresenter()
        {
            _singleActiveChildPresenterModule = new SingleActiveChildPresenterModule(this);
        }

        protected override async UniTask InitializeUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _singleActiveChildPresenterModule.Initialize(disposables);

            await Show(cancellationToken);
        }
    }
}
#endif
#endif
