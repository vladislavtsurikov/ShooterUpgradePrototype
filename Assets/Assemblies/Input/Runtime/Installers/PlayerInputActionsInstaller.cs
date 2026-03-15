using AutoStrike.Input.Generated;
using AutoStrike.Input.Services;
using UnityEngine;
using Zenject;

namespace AutoStrike.Input.Installers
{
    [AddComponentMenu("AutoStrike/Input/Player Input Actions Installer")]
    public sealed class PlayerInputActionsInstaller : MonoInstaller
    {
        private PlayerInputActions _playerInputActions;
        private InputModeService _inputModeService;
        private MobileInputVirtualGamepad _mobileInputVirtualGamepad;

        public override void InstallBindings()
        {
            _playerInputActions = new PlayerInputActions();
            _mobileInputVirtualGamepad = new MobileInputVirtualGamepad();
            _inputModeService = new InputModeService(_mobileInputVirtualGamepad, InputModePreferences.CurrentPreference);

            _playerInputActions.Enable();

            Container.Bind<PlayerInputActions>().FromInstance(_playerInputActions).AsSingle();
            Container.Bind<InputModeService>().FromInstance(_inputModeService).AsSingle();
            Container.Bind<MobileInputVirtualGamepad>().FromInstance(_mobileInputVirtualGamepad).AsSingle();

            InputModePreferences.PreferenceChanged += HandlePreferenceChanged;
        }

        private void OnDestroy()
        {
            InputModePreferences.PreferenceChanged -= HandlePreferenceChanged;
            _inputModeService?.Dispose();
            _inputModeService = null;

            _mobileInputVirtualGamepad?.Dispose();
            _mobileInputVirtualGamepad = null;

            if (_playerInputActions == null)
            {
                return;
            }

            _playerInputActions.Disable();
            _playerInputActions.Dispose();
            _playerInputActions = null;
        }

        private void HandlePreferenceChanged(string preference) =>
            _inputModeService?.SetPreferredMode(preference);
    }
}
