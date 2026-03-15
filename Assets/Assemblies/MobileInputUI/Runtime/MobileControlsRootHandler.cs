#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
#if UI_SYSTEM_UNIRX
#if UI_SYSTEM_ZENJECT
using System.Threading;
using AutoStrike.Input.Services;
using AutoStrike.Input.Services.States;
using AutoStrike.MobileInputUI.Loaders;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.UIElements;
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

        protected override async UniTask InitializeUIHandler(
            CancellationToken cancellationToken,
            CompositeDisposable disposables)
        {
            await Show(cancellationToken);

            _inputModeService.CurrentState
                .Select(state => state is TouchscreenInputModeState)
                .DistinctUntilChanged()
                .Subscribe(SetVisible)
                .AddTo(disposables);
        }

        protected override string SpawnedRootName => "AutoStrikeMobileControlsHUD";

        private void SetVisible(bool isVisible)
        {
            if (SpawnedRoot == null)
            {
                return;
            }

            if (isVisible)
            {
                SpawnedRoot.style.display = StyleKeyword.Null;
                return;
            }

            SpawnedRoot.style.display = DisplayStyle.None;
        }
    }
}
#endif
#endif
#endif
