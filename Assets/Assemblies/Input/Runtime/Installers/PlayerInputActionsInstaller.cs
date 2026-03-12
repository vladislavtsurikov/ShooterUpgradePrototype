using AutoStrike.Input.Generated;
using UnityEngine;
using Zenject;

namespace AutoStrike.Input.Installers
{
    [AddComponentMenu("AutoStrike/Input/Player Input Actions Installer")]
    public sealed class PlayerInputActionsInstaller : MonoInstaller
    {
        private PlayerInputActions _playerInputActions;

        public override void InstallBindings()
        {
            _playerInputActions = new PlayerInputActions();
            _playerInputActions.Enable();

            Container.Bind<PlayerInputActions>().FromInstance(_playerInputActions).AsSingle();
        }

        private void OnDestroy()
        {
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
