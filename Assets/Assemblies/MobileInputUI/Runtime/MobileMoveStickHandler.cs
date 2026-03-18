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
    public sealed class MobileMoveStickHandler : ParentBoundUIToolkitHandler
    {
        private readonly MobileInputStateService _mobileInputStateService;
        private readonly InputModeService _inputModeService;

        public MobileMoveStickHandler(
            DiContainer container,
            MobileInputStateService mobileInputStateService,
            InputModeService inputModeService) : base(container)
        {
            _mobileInputStateService = mobileInputStateService;
            _inputModeService = inputModeService;
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            MobileMoveStickView view = GetView<MobileMoveStickView>(nameof(MobileMoveStickView));

            view.OnInputChanged
                .Subscribe(direction =>
                {
                    _mobileInputStateService.SetMove(direction, true);
                    _inputModeService.ReportTouchInput();
                })
                .AddTo(disposables);

            view.OnReleased
                .Subscribe(_ => _mobileInputStateService.ResetMove())
                .AddTo(disposables);

            Disposable.Create(() => _mobileInputStateService.ResetMove())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
