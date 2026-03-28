#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using AutoStrike.Input.InputMode.Runtime;
using Cysharp.Threading.Tasks;
using UIRootSystem.Runtime;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace AutoStrike.MobileInputUI.MobileInputUI.Runtime
{
    [SceneFilter("Battle")]
    [UIParent(typeof(Root), RootSlots.HUD)]
    public sealed class MobileControlsRootPresenter : UIToolkitUIPresenter
    {
        private readonly InputModeService _inputModeService;

        public MobileControlsRootPresenter(
            MobileControlsLayoutLoader loader,
            InputModeService inputModeService)
            : base(loader)
        {
            _inputModeService = inputModeService;
        }

        protected override UniTask InitializeUIPresenter(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _inputModeService.CurrentInputSource
                .Select(state => state == InputSource.Touch)
                .DistinctUntilChanged()
                .Subscribe(isTouchscreen => ToggleVisibilityAsync(isTouchscreen, cancellationToken).Forget())
                .AddTo(disposables);

            return UniTask.CompletedTask;
        }

        protected override string SpawnedRootName => "AutoStrikeMobileControlsHUD";

        private async UniTaskVoid ToggleVisibilityAsync(bool isVisible, CancellationToken cancellationToken)
        {
            if (isVisible)
            {
                await Show(cancellationToken);
                return;
            }

            await Hide(cancellationToken);
        }
    }
}
#endif
#endif
#endif
