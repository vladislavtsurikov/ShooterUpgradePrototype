using System;
using UniRx;
using UnityEngine;

namespace AutoStrike.Input.Services
{
    public sealed class MobileInputStateService : IDisposable
    {
        private readonly ReactiveProperty<Vector2> _moveDirection = new(Vector2.zero);
        private readonly ReactiveProperty<bool> _isMoveStickActive = new(false);
        private readonly ReactiveProperty<bool> _isFireButtonPressed = new(false);

        public IReadOnlyReactiveProperty<Vector2> MoveDirection => _moveDirection;
        public IReadOnlyReactiveProperty<bool> IsMoveStickActive => _isMoveStickActive;
        public IReadOnlyReactiveProperty<bool> IsFireButtonPressed => _isFireButtonPressed;

        public void SetMove(Vector2 moveDirection, bool isPressed)
        {
            _moveDirection.Value = isPressed ? Vector2.ClampMagnitude(moveDirection, 1f) : Vector2.zero;
            _isMoveStickActive.Value = isPressed;
        }

        public void ResetMove() => SetMove(Vector2.zero, false);

        public void SetFirePressed(bool isPressed) => _isFireButtonPressed.Value = isPressed;

        public void Dispose()
        {
            _moveDirection.Dispose();
            _isMoveStickActive.Dispose();
            _isFireButtonPressed.Dispose();
        }
    }
}
