#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using AutoStrike.Input.Services;
using AutoStrike.Input.Services.States;
using AutoStrike.MobileInputUI.Loaders;
using Cysharp.Threading.Tasks;
using UniRx;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.UIRootSystem.Runtime.UIToolkitIntegration;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace AutoStrike.MobileInputUI.Handlers
{
    [SceneFilter("Battle")]
    [UIParent(typeof(Root), RootSlots.HudRoot)]
    public sealed class MobileControlsRootHandler : UIToolkitUIHandler
    {
        private readonly InputModeService _inputModeService;

        public MobileControlsRootHandler(
            DiContainer container,
            MobileControlsLayoutLoader loader,
            InputModeService inputModeService)
            : base(container, loader)
        {
            _inputModeService = inputModeService;
        }

        protected override UniTask InitializeUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            _inputModeService.CurrentState
                .Select(state => state is TouchscreenInputModeState)
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
