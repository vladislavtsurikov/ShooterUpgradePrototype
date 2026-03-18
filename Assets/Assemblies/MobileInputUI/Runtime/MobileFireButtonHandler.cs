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

namespace AutoStrike.MobileInputUI.Handlers
{
    [SceneFilter("Battle")]
    [UIParent(typeof(MobileControlsRootHandler))]
    public sealed class MobileFireButtonHandler : ParentBoundUIToolkitHandler
    {
        private readonly MobileVirtualInputService _mobileVirtualInputService;
        private readonly InputModeService _inputModeService;

        public MobileFireButtonHandler(
            DiContainer container,
            MobileVirtualInputService mobileVirtualInputService,
            InputModeService inputModeService) : base(container)
        {
            _mobileVirtualInputService = mobileVirtualInputService;
            _inputModeService = inputModeService;
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            MobileFireButtonView view = GetView<MobileFireButtonView>(nameof(MobileFireButtonView));

            view.OnPressedChanged
                .DistinctUntilChanged()
                .Subscribe(isPressed =>
                {
                    _mobileVirtualInputService.SetFirePressed(isPressed);
                    if (isPressed)
                    {
                        _inputModeService.ReportTouchInput();
                    }
                })
                .AddTo(disposables);

            Disposable.Create(() => _mobileVirtualInputService.SetFirePressed(false))
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
