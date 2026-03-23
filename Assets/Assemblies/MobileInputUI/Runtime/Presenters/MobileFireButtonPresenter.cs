#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using AutoStrike.Input.FPSInput.Runtime;
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
    public sealed class MobileFireButtonPresenter : ParentBoundUIToolkitHandler
    {
        private readonly MobileVirtualInputService _mobileVirtualInputService;

        public MobileFireButtonPresenter(
            DiContainer container,
            MobileVirtualInputService mobileVirtualInputService) : base(container)
        {
            _mobileVirtualInputService = mobileVirtualInputService;
        }

        protected override UniTask InitializeUIHandler(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            MobileFireButtonView view = GetView<MobileFireButtonView>(nameof(MobileFireButtonView));

            view.OnPressedChanged
                .DistinctUntilChanged()
                .Subscribe(isPressed =>
                {
                    _mobileVirtualInputService.SetFirePressed(isPressed);
                })
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }
    }
}
#endif
#endif
