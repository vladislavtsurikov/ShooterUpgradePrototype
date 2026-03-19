#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using AutoStrike.Input.Services;
using AutoStrike.MobileInputUI.Views;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace AutoStrike.MobileInputUI.Presenters
{
    [SceneFilter("Battle")]
    [UIParent(typeof(MobileControlsRootPresenter))]
    public sealed class MobileMoveStickPresenter : ParentBoundUIToolkitHandler
    {
        private readonly MobileVirtualInputService _mobileVirtualInputService;
        private readonly InputModeService _inputModeService;

        public MobileMoveStickPresenter(
            DiContainer container,
            MobileVirtualInputService mobileVirtualInputService,
            InputModeService inputModeService) : base(container)
        {
            _mobileVirtualInputService = mobileVirtualInputService;
            _inputModeService = inputModeService;
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            MobileMoveStickView view = GetView<MobileMoveStickView>(nameof(MobileMoveStickView));

            view.OnInputChanged
                .Subscribe(direction =>
                {
                    _mobileVirtualInputService.SetMove(direction, true);
                    _inputModeService.ReportTouchInput();
                })
                .AddTo(disposables);

            view.OnReleased
                .Subscribe(_ => _mobileVirtualInputService.ResetMove())
                .AddTo(disposables);

            Disposable.Create(() => _mobileVirtualInputService.ResetMove())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
