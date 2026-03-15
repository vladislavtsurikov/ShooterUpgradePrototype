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
        private readonly MobileInputVirtualGamepad _mobileInputVirtualGamepad;

        public MobileFireButtonHandler(
            DiContainer container,
            MobileInputVirtualGamepad mobileInputVirtualGamepad) : base(container)
        {
            _mobileInputVirtualGamepad = mobileInputVirtualGamepad;
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            MobileFireButtonView view = GetUIComponent<MobileFireButtonView>(nameof(MobileFireButtonView));

            view.OnPressedChanged
                .DistinctUntilChanged()
                .Subscribe(_mobileInputVirtualGamepad.SetFirePressed)
                .AddTo(disposables);

            Disposable.Create(() => _mobileInputVirtualGamepad.SetFirePressed(false))
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
