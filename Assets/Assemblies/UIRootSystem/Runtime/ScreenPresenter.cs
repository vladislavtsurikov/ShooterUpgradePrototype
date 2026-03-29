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
        private UIPresenter _activeScreen;

        protected override async UniTask InitializeUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            OnUIPresenterAfterShow += OnChildScreenShown;
            OnUIPresenterAfterHide += OnChildScreenHidden;
            disposables.Add(Disposable.Create(() =>
            {
                OnUIPresenterAfterShow -= OnChildScreenShown;
                OnUIPresenterAfterHide -= OnChildScreenHidden;
            }));

            await Show(cancellationToken);
        }

        private void OnChildScreenShown(UIPresenter presenter)
        {
            if (presenter.Parent != this)
            {
                return;
            }

            if (_activeScreen != null && _activeScreen != presenter)
            {
                _activeScreen.Hide(CancellationToken.None).Forget();
            }

            _activeScreen = presenter;
        }

        private void OnChildScreenHidden(UIPresenter presenter)
        {
            if (_activeScreen == presenter)
            {
                _activeScreen = null;
            }
        }
    }
}
#endif
#endif
