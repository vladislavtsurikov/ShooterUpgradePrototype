using System;
using UniRx;
using UnityEngine.InputSystem;
using AutoStrike.Input.Services.States;

namespace AutoStrike.Input.Services
{
    public sealed class InputModeService : IDisposable
    {
        private readonly ReactiveProperty<InputModeState> _currentState;
        private readonly InputModeStateRegistry _stateRegistry;
        private InputDevice _lastUsedDevice;

        public InputModeService(InputModeStateRegistry stateRegistry)
        {
            _stateRegistry = stateRegistry;
            _currentState = new ReactiveProperty<InputModeState>();

            InputSystem.onDeviceChange += HandleDeviceChange;

            RefreshCurrentState();
        }

        public IReadOnlyReactiveProperty<InputModeState> CurrentState => _currentState;

        public void ReportDevice(InputDevice device)
        {
            if (device == null)
            {
                return;
            }

            _lastUsedDevice = device;
            RefreshCurrentState();
        }

        public void ReportTouchInput() => ReportDevice(Touchscreen.current);

        public void Dispose()
        {
            for (int i = 0; i < _stateRegistry.All.Count; i++)
            {
                _stateRegistry.All[i].SetActive(false);
            }

            _currentState.Dispose();
            InputSystem.onDeviceChange -= HandleDeviceChange;
            _stateRegistry.Dispose();
        }

        private void RefreshCurrentState()
        {
            InputModeState nextState = _stateRegistry.GetHighestPriorityState(_lastUsedDevice);
            SwitchState(nextState);
        }

        private void SwitchState(InputModeState nextState)
        {
            if (ReferenceEquals(_currentState.Value, nextState))
            {
                return;
            }

            _currentState.Value = nextState;
        }

        private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                case InputDeviceChange.Removed:
                case InputDeviceChange.Disconnected:
                case InputDeviceChange.Reconnected:
                    if (ReferenceEquals(_lastUsedDevice, device) && change is InputDeviceChange.Removed or InputDeviceChange.Disconnected)
                    {
                        _lastUsedDevice = null;
                    }

                    RefreshCurrentState();
                    break;
            }
        }
    }
}
