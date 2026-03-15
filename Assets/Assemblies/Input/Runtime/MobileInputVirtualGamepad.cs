using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace AutoStrike.Input.Services
{
    public sealed class MobileInputVirtualGamepad : IDisposable
    {
        private readonly Gamepad _virtualGamepad;

        public MobileInputVirtualGamepad()
        {
            _virtualGamepad = InputSystem.AddDevice<Gamepad>("Mobile Input Virtual Gamepad");
            ResetMove();
            SetFirePressed(false);
        }

        public bool IsMoveStickActive { get; private set; }
        public bool IsFireButtonPressed { get; private set; }

        public void SetMove(Vector2 moveDirection, bool isPressed)
        {
            IsMoveStickActive = isPressed;
            Vector2 clampedDirection = Vector2.ClampMagnitude(moveDirection, 1f);
            InputState.Change(_virtualGamepad.leftStick, clampedDirection);
        }

        public void ResetMove() => SetMove(Vector2.zero, false);

        public void SetFirePressed(bool isPressed)
        {
            IsFireButtonPressed = isPressed;
            InputState.Change(_virtualGamepad.buttonSouth, isPressed ? 1f : 0f);
        }

        public bool Owns(InputDevice device) => ReferenceEquals(device, _virtualGamepad);

        public void Dispose()
        {
            ResetMove();
            SetFirePressed(false);

            if (_virtualGamepad != null && _virtualGamepad.added)
            {
                InputSystem.RemoveDevice(_virtualGamepad);
            }
        }
    }
}
