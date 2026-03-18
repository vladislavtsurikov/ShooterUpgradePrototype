using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AutoStrike.Input.Services
{
    public sealed class MobileVirtualInputService : IDisposable
    {
        private const string VirtualDeviceName = "AutoStrike.MobileVirtualGamepad";

        private Gamepad _virtualGamepad;

        public bool IsMoveStickActive { get; private set; }
        public bool IsFireButtonPressed { get; private set; }

        public void SetMove(Vector2 moveDirection, bool isPressed)
        {
            EnsureDevice();

            Vector2 clamped = isPressed ? Vector2.ClampMagnitude(moveDirection, 1f) : Vector2.zero;
            IsMoveStickActive = isPressed;
            InputSystem.QueueDeltaStateEvent(_virtualGamepad.leftStick, clamped);
        }

        public void ResetMove() => SetMove(Vector2.zero, false);

        public void SetFirePressed(bool isPressed)
        {
            EnsureDevice();

            IsFireButtonPressed = isPressed;
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

            IsMoveStickActive = false;
            IsFireButtonPressed = false;
        }

        private void EnsureDevice()
        {
            if (_virtualGamepad != null)
            {
                return;
            }

            _virtualGamepad = InputSystem.AddDevice<Gamepad>(VirtualDeviceName);
        }
    }
}
