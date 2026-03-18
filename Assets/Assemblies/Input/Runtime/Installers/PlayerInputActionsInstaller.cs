using AutoStrike.Input.Generated;
using AutoStrike.Input.Services;
using AutoStrike.Input.Services.States;
using UnityEngine;
using Zenject;

namespace AutoStrike.Input.Installers
{
    [AddComponentMenu("AutoStrike/Input/Player Input Actions Installer")]
    public sealed class PlayerInputActionsInstaller : MonoInstaller
    {
        private PlayerInputActions _playerInputActions;
        private InputModeService _inputModeService;
        private MobileVirtualInputService _mobileVirtualInputService;

        public override void InstallBindings()
        {
            _playerInputActions = new PlayerInputActions();
            _inputModeService = new InputModeService(new InputModeStateRegistry());
            _mobileVirtualInputService = new MobileVirtualInputService();

            _playerInputActions.Enable();

            Container.Bind<PlayerInputActions>().FromInstance(_playerInputActions).AsSingle();
            Container.Bind<InputModeService>().FromInstance(_inputModeService).AsSingle();
            Container.Bind<MobileVirtualInputService>().FromInstance(_mobileVirtualInputService).AsSingle();
        }

        private void OnDestroy()
        {
            _inputModeService?.Dispose();
            _inputModeService = null;

            _mobileVirtualInputService?.Dispose();
            _mobileVirtualInputService = null;

            if (_playerInputActions == null)
            {
                return;
            }

            _playerInputActions.Disable();
            _playerInputActions.Dispose();
            _playerInputActions = null;
        }
    }
}
