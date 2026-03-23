using System;
using AutoStrike.Input.InputMode.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace AutoStrike.Input.FPSInput.Runtime
{
    public sealed class MobileVirtualInputService : IDisposable
    {
        private Gamepad _virtualGamepad;
        private Vector2 _moveDirection;
        private bool _isFirePressed;

        public void SetMove(Vector2 moveDirection, bool isPressed)
        {
            EnsureDevice();

            _moveDirection = isPressed ? Vector2.ClampMagnitude(moveDirection, 1f) : Vector2.zero;
            ApplyState();
        }

        public void ResetMove() => SetMove(Vector2.zero, false);

        public void SetFirePressed(bool isPressed)
        {
            EnsureDevice();

            _isFirePressed = isPressed;
            ApplyState();
        }

        public void Dispose()
        {
            if (_virtualGamepad != null)
            {
                _moveDirection = Vector2.zero;
                _isFirePressed = false;
                ApplyState();
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

        private void ApplyState()
        {
            GamepadState state = new GamepadState
            {
                leftStick = _moveDirection
            };

            if (_isFirePressed)
            {
                state.buttons |= 1u << (int)GamepadButton.South;
            }

            InputSystem.QueueStateEvent(_virtualGamepad, state);
        }
    }
}
