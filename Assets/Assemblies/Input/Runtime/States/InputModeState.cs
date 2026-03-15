using System;
using UniRx;
using UnityEngine.InputSystem;

namespace AutoStrike.Input.Services.States
{
    public abstract class InputModeState : IDisposable
    {
        protected InputModeState(MobileInputVirtualGamepad mobileInputVirtualGamepad)
        {
            MobileInputVirtualGamepad = mobileInputVirtualGamepad;
            ShouldActive = new ReactiveProperty<bool>();
            IsActive = new ReactiveProperty<bool>();
        }

        protected MobileInputVirtualGamepad MobileInputVirtualGamepad { get; }

        public ReactiveProperty<bool> ShouldActive { get; }
        public ReactiveProperty<bool> IsActive { get; }
        public abstract string Id { get; }

        public void Refresh(
            string preferredMode,
            InputDevice lastUsedDevice,
            string defaultMode)
        {
            bool shouldActive = preferredMode == InputModeIds.Auto
                ? ResolveAutoActive(lastUsedDevice, defaultMode)
                : MatchesPreference(preferredMode);

            if (ShouldActive.Value != shouldActive)
            {
                ShouldActive.Value = shouldActive;
            }
        }

        public void SetActive(bool isActive)
        {
            if (IsActive.Value != isActive)
            {
                IsActive.Value = isActive;
            }
        }

        public virtual void Dispose()
        {
            ShouldActive.Dispose();
            IsActive.Dispose();
        }

        protected virtual bool MatchesPreference(string preferredMode) =>
            preferredMode == Id;

        protected virtual bool ResolveAutoActive(InputDevice lastUsedDevice, string defaultMode)
        {
            if (lastUsedDevice != null)
            {
                return MatchesDevice(lastUsedDevice);
            }

            return Id == defaultMode;
        }

        protected abstract bool MatchesDevice(InputDevice device);
    }
}
