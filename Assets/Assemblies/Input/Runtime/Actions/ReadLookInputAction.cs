using AutoStrike.Input.Data;
using AutoStrike.Input.Generated;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Zenject;

namespace AutoStrike.Input.Actions
{
    [RequiresData(typeof(LookInputData))]
    [Name("AutoStrike.Input/Actions/ReadLookInput")]
    public sealed class ReadLookInputAction : EntityMonoBehaviourAction
    {
        [Inject]
        private PlayerInputActions _playerInputActions;

        private LookInputData _lookInputData;
        private Vector2 _previousPointerPosition;
        private bool _hasPreviousPointerPosition;

        protected override void OnEnable()
        {
            _lookInputData = Entity.GetData<LookInputData>();
            ResetLookInput();

            InputAction lookAction = _playerInputActions.Player.Look;

            lookAction.started += OnLookChanged;
            lookAction.performed += OnLookChanged;
            lookAction.canceled += OnLookChanged;
        }

        protected override void OnDisable()
        {
            InputAction lookAction = _playerInputActions.Player.Look;

            lookAction.started -= OnLookChanged;
            lookAction.performed -= OnLookChanged;
            lookAction.canceled -= OnLookChanged;

            ResetLookInput();
        }

        private void OnLookChanged(InputAction.CallbackContext context)
        {
            if (context.control is StickControl)
            {
                ApplyLookRate(context);
                return;
            }

            ApplyLookDelta(context);
        }

        private void ApplyLookRate(InputAction.CallbackContext context)
        {
            _lookInputData.LookDelta.Value = Vector2.zero;

            if (context.canceled)
            {
                _lookInputData.LookRate.Value = Vector2.zero;
                return;
            }

            _lookInputData.LookRate.Value = context.ReadValue<Vector2>();
        }

        private void ApplyLookDelta(InputAction.CallbackContext context)
        {
            _lookInputData.LookRate.Value = Vector2.zero;

            if (IsDeltaControl(context.control))
            {
                _lookInputData.LookDelta.Value = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
                return;
            }

            if (context.canceled)
            {
                _lookInputData.LookDelta.Value = Vector2.zero;
                _hasPreviousPointerPosition = false;
                return;
            }

            Vector2 currentPointerPosition = context.ReadValue<Vector2>();
            if (!_hasPreviousPointerPosition)
            {
                _previousPointerPosition = currentPointerPosition;
                _hasPreviousPointerPosition = true;
                _lookInputData.LookDelta.Value = Vector2.zero;
                return;
            }

            _lookInputData.LookDelta.Value = currentPointerPosition - _previousPointerPosition;
            _previousPointerPosition = currentPointerPosition;
        }

        private void ResetLookInput()
        {
            _hasPreviousPointerPosition = false;
            _previousPointerPosition = Vector2.zero;

            if (_lookInputData == null)
            {
                return;
            }

            _lookInputData.LookDelta.Value = Vector2.zero;
            _lookInputData.LookRate.Value = Vector2.zero;
        }

        private static bool IsDeltaControl(InputControl control)
        {
            return control != null && control.name == "delta";
        }
    }
}
