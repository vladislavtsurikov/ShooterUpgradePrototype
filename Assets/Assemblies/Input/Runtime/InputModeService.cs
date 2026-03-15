using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using AutoStrike.Input.Services.States;

namespace AutoStrike.Input.Services
{
    public sealed class InputModeService : IDisposable
    {
        private readonly CompositeDisposable _subscriptions = new();
        private readonly List<InputModeState> _states;
        private InputDevice _lastUsedDevice;

        public InputModeService(
            MobileInputVirtualGamepad mobileInputVirtualGamepad,
            string initialPreference)
        {
            _states = new List<InputModeState>
            {
                new TouchscreenInputModeState(mobileInputVirtualGamepad),
                new GamepadInputModeState(mobileInputVirtualGamepad),
                new KeyboardMouseInputModeState(mobileInputVirtualGamepad)
            };

            PreferredMode = new ReactiveProperty<string>(InputModeIds.NormalizePreference(initialPreference));
            CurrentState = new ReactiveProperty<InputModeState>(GetDefaultState());

            SubscribeToStates();
            RefreshStates();
        }

        public ReactiveProperty<string> PreferredMode { get; }
        public ReactiveProperty<InputModeState> CurrentState { get; }

        public void SetPreferredMode(string preference)
        {
            string normalizedPreference = InputModeIds.NormalizePreference(preference);
            if (PreferredMode.Value == normalizedPreference)
            {
                return;
            }

            PreferredMode.Value = normalizedPreference;
            RefreshStates();
        }

        public void ReportDevice(InputDevice device)
        {
            if (device == null || PreferredMode.Value != InputModeIds.Auto)
            {
                return;
            }

            _lastUsedDevice = device;
            RefreshStates();
        }

        public void Dispose()
        {
            _subscriptions.Dispose();

            foreach (InputModeState state in _states)
            {
                state.Dispose();
            }
        }

        private void SubscribeToStates()
        {
            foreach (InputModeState state in _states)
            {
                state.ShouldActive
                    .Subscribe(_ => ResolveActiveState())
                    .AddTo(_subscriptions);
            }
        }

        private void RefreshStates()
        {
            string defaultModeId = ResolveDefaultModeId();

            foreach (InputModeState state in _states)
            {
                state.Refresh(PreferredMode.Value, _lastUsedDevice, defaultModeId);
            }

            ResolveActiveState();
        }

        private void ResolveActiveState()
        {
            InputModeState activeState = null;

            foreach (InputModeState state in _states)
            {
                if (state.ShouldActive.Value)
                {
                    activeState = state;
                    break;
                }
            }

            activeState ??= GetFallbackState();

            foreach (InputModeState state in _states)
            {
                state.SetActive(ReferenceEquals(state, activeState));
            }

            if (!ReferenceEquals(CurrentState.Value, activeState))
            {
                CurrentState.Value = activeState;
            }
        }

        private InputModeState GetFallbackState()
        {
            string defaultModeId = ResolveDefaultModeId();

            foreach (InputModeState state in _states)
            {
                if (state.Id == defaultModeId)
                {
                    return state;
                }
            }

            return _states[0];
        }

        private InputModeState GetDefaultState()
        {
            string defaultModeId = ResolveDefaultModeId();

            foreach (InputModeState state in _states)
            {
                if (state.Id == defaultModeId)
                {
                    return state;
                }
            }

            return _states[0];
        }

        private static string ResolveDefaultModeId()
        {
            if (Application.isMobilePlatform || Touchscreen.current != null)
            {
                return InputModeIds.Touch;
            }

            return InputModeIds.KeyboardMouse;
        }
    }
}
