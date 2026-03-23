using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AutoStrike.Input.InputMode.Runtime
{
    public sealed class InputModeService : IDisposable
    {
        private readonly ReactiveProperty<InputSource> _currentInputSource;
        private InputDevice _lastUsedDevice;

        public IReadOnlyReactiveProperty<InputSource> CurrentInputSource => _currentInputSource;

        public InputModeService()
        {
            _currentInputSource = new ReactiveProperty<InputSource>(InputSource.KeyboardMouse);

            InputSystem.onDeviceChange += HandleDeviceChange;

            RefreshCurrentSource();
        }

        public void ReportDevice(InputDevice device)
        {
            _lastUsedDevice = device;
            RefreshCurrentSource();
        }

        public void ReportTouchInput() => ReportDevice(Touchscreen.current);

        public void Dispose()
        {
            _currentInputSource.Dispose();
            InputSystem.onDeviceChange -= HandleDeviceChange;
        }

        private void RefreshCurrentSource()
        {
            if (TryGetSourceFromLastUsedDevice(_lastUsedDevice, out InputSource source))
            {
                SetCurrentSource(source);
                return;
            }

            if (TryGetFallbackSourceFromConnectedDevices(out source))
            {
                SetCurrentSource(source);
                return;
            }

            SetCurrentSource(GetPlatformDefaultSource());
        }

        private void SetCurrentSource(InputSource source)
        {
            if (_currentInputSource.Value == source)
            {
                return;
            }

            _currentInputSource.Value = source;
        }

        private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:
                case InputDeviceChange.Reconnected:
                    if (ShouldClearLastUsedDevice(_lastUsedDevice, device, change))
                    {
                        _lastUsedDevice = null;
                    }

                    RefreshCurrentSource();
                    break;
            }
        }

        private static bool ShouldClearLastUsedDevice(InputDevice lastUsedDevice, InputDevice changedDevice, InputDeviceChange change)
        {
            return ReferenceEquals(lastUsedDevice, changedDevice)
                && change is InputDeviceChange.Removed or InputDeviceChange.Disconnected;
        }

        private static bool TryGetSourceFromLastUsedDevice(InputDevice device, out InputSource source)
        {
            if (IsPhysicalGamepad(device))
            {
                source = InputSource.Gamepad;
                return true;
            }

            if (device is Touchscreen)
            {
                source = InputSource.Touch;
                return true;
            }

            if (device is Keyboard or Mouse)
            {
                source = InputSource.KeyboardMouse;
                return true;
            }

            source = default;
            return false;
        }

        private static bool TryGetFallbackSourceFromConnectedDevices(out InputSource source)
        {
            if (HasPhysicalGamepad())
            {
                source = InputSource.Gamepad;
                return true;
            }

            source = default;
            return false;
        }

        private static InputSource GetPlatformDefaultSource()
        {
            return Application.isMobilePlatform
                ? InputSource.Touch
                : InputSource.KeyboardMouse;
        }

        private static bool HasPhysicalGamepad()
        {
            for (int i = 0; i < Gamepad.all.Count; i++)
            {
                if (IsPhysicalGamepad(Gamepad.all[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPhysicalGamepad(InputDevice device) =>
            device is Gamepad && !IsMobileVirtualGamepad(device);

        private static bool IsMobileVirtualGamepad(InputDevice device) =>
            device.description.product == InputDeviceConstants.MobileVirtualGamepadName;
    }
}
