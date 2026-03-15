using AutoStrike.Input.Data;
using AutoStrike.Input.Generated;
using AutoStrike.Input.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Zenject;

namespace AutoStrike.Input.Actions
{
    [RequiresData(typeof(MoveInputData))]
    [Name("AutoStrike.Input/Actions/ReadMoveInput")]
    public sealed class ReadMoveInputAction : EntityMonoBehaviourAction
    {
        [Inject]
        private PlayerInputActions _playerInputActions;

        [Inject]
        private InputModeService _inputModeService;

        private MoveInputData _moveInputData;

        protected override void OnEnable()
        {
            _moveInputData = Entity.GetData<MoveInputData>();
            InputAction moveAction = _playerInputActions.Player.Move;

            moveAction.started += OnMoveChanged;
            moveAction.performed += OnMoveChanged;
            moveAction.canceled += OnMoveChanged;

            _moveInputData.MoveDirection.Value = moveAction.ReadValue<Vector2>();
        }

        protected override void OnDisable()
        {
            InputAction moveAction = _playerInputActions.Player.Move;

            moveAction.started -= OnMoveChanged;
            moveAction.performed -= OnMoveChanged;
            moveAction.canceled -= OnMoveChanged;

            if (_moveInputData != null)
            {
                _moveInputData.MoveDirection.Value = Vector2.zero;
            }
        }

        private void OnMoveChanged(InputAction.CallbackContext context)
        {
            _inputModeService.ReportDevice(context.control?.device);
            _moveInputData.MoveDirection.Value = context.ReadValue<Vector2>();
        }
    }
}
