using System;
using UniRx;
using UnityEngine.InputSystem;

namespace AutoStrike.Input.Services.States
{
    public abstract class InputModeState : IDisposable
    {
        private readonly ReactiveProperty<bool> _isActive = new();

        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;
        public abstract string Name { get; }
        public abstract int GetPriority(InputDevice lastUsedDevice);

        internal void SetActive(bool isActive)
        {
            if (_isActive.Value == isActive)
            {
                return;
            }

            _isActive.Value = isActive;
        }

        public virtual void Dispose() => _isActive.Dispose();
    }
}
