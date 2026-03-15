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
        private readonly MobileInputVirtualGamepad _mobileInputVirtualGamepad;

        public MobileMoveStickHandler(
            DiContainer container,
            MobileInputVirtualGamepad mobileInputVirtualGamepad) : base(container)
        {
            _mobileInputVirtualGamepad = mobileInputVirtualGamepad;
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            MobileMoveStickView view = GetUIComponent<MobileMoveStickView>(nameof(MobileMoveStickView));

            view.OnInputChanged
                .Subscribe(direction => _mobileInputVirtualGamepad.SetMove(direction, true))
                .AddTo(disposables);

            view.OnReleased
                .Subscribe(_ => _mobileInputVirtualGamepad.ResetMove())
                .AddTo(disposables);

            Disposable.Create(() => _mobileInputVirtualGamepad.ResetMove())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
