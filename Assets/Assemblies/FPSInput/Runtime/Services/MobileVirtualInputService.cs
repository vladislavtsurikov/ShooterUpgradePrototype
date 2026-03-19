using System;
using AutoStrike.Input.InputMode.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AutoStrike.Input.FPSInput.Runtime
{
    public sealed class MobileVirtualInputService : IDisposable
    {
        private Gamepad _virtualGamepad;

        public void SetMove(Vector2 moveDirection, bool isPressed)
        {
            EnsureDevice();

            Vector2 clamped = isPressed ? Vector2.ClampMagnitude(moveDirection, 1f) : Vector2.zero;
            InputSystem.QueueDeltaStateEvent(_virtualGamepad.leftStick, clamped);
        }

        public void ResetMove() => SetMove(Vector2.zero, false);

        public void SetFirePressed(bool isPressed)
        {
            EnsureDevice();

            InputSystem.QueueDeltaStateEvent(_virtualGamepad.buttonSouth, isPressed ? 1f : 0f);
        }

        public void Dispose()
        {
            if (_virtualGamepad != null)
            {
                InputSystem.QueueDeltaStateEvent(_virtualGamepad.leftStick, Vector2.zero);
                InputSystem.QueueDeltaStateEvent(_virtualGamepad.buttonSouth, 0f);
                InputSystem.RemoveDevice(_virtualGamepad);
                _virtualGamepad = null;
            }
        }

        private void EnsureDevice()
        {
            if (_virtualGamepad != null)
            {
                return;
            }

            _virtualGamepad = InputSystem.AddDevice<Gamepad>(InputDeviceConstants.MobileVirtualGamepadName);
        }
    }
}
