#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using AutoStrike.Input.FPSInput.Runtime;
using AutoStrike.Input.InputMode.Runtime;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace AutoStrike.MobileInputUI.MobileInputUI.Runtime
{
    [SceneFilter("Battle")]
    [UIParent(typeof(MobileControlsRootPresenter))]
    public sealed class MobileMoveStickPresenter : UIToolkitUIHandler
    {
        private readonly MobileVirtualInputService _mobileVirtualInputService;
        private readonly InputModeService _inputModeService;

        public MobileMoveStickPresenter(
            MobileVirtualInputService mobileVirtualInputService,
            InputModeService inputModeService)
            : base()
        {
            _mobileVirtualInputService = mobileVirtualInputService;
            _inputModeService = inputModeService;
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            MobileMoveStickView view = ViewResolver.GetView<MobileMoveStickView>(nameof(MobileMoveStickView));

            view.OnPointerChanged
                .Subscribe(pointerData =>
                {
                    Vector2 knobOffset = Vector2.ClampMagnitude(pointerData.LocalPosition - pointerData.PadCenter, pointerData.Radius);
                    Vector2 direction = pointerData.Radius <= Mathf.Epsilon
                        ? Vector2.zero
                        : knobOffset / pointerData.Radius;

                    direction.y = -direction.y;

                    view.SetKnobOffset(knobOffset);
                    _mobileVirtualInputService.SetMove(direction, true);
                    _inputModeService.ReportTouchInput();
                })
                .AddTo(disposables);

            view.OnReleased
                .Subscribe(_ =>
                {
                    view.ResetKnob();
                    _mobileVirtualInputService.ResetMove();
                })
                .AddTo(disposables);

            Disposable.Create(() =>
                {
                    view.ResetKnob();
                    _mobileVirtualInputService.ResetMove();
                })
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
