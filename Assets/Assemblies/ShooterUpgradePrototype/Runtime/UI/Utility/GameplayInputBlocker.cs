using AutoStrike.Input.Generated;
using UnityEngine;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    public sealed class GameplayInputBlocker
    {
        private readonly PlayerInputActions _playerInputActions;
        private bool _cursorWasVisible;
        private CursorLockMode _previousCursorLockMode;

        public GameplayInputBlocker(PlayerInputActions playerInputActions)
        {
            _playerInputActions = playerInputActions;
        }

        public void DisableGameplayInput()
        {
            _playerInputActions.Player.Disable();

            _cursorWasVisible = Cursor.visible;
            _previousCursorLockMode = Cursor.lockState;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void EnableGameplayInput()
        {
            _playerInputActions.Player.Enable();

            Cursor.visible = _cursorWasVisible;
            Cursor.lockState = _previousCursorLockMode;
        }
    }
}
