using AutoStrike.Input.Data;
using AutoStrike.Input.Generated;
using AutoStrike.Input.Services;
using UniRx;
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

        [Inject]
        private MobileInputStateService _mobileInputStateService;

        private MoveInputData _moveInputData;
        private readonly CompositeDisposable _disposables = new();
        private Vector2 _actionMoveDirection;
        private Vector2 _mobileMoveDirection;
        private bool _isMobileMoveActive;

        protected override void OnEnable()
        {
            _moveInputData = Entity.GetData<MoveInputData>();
            InputAction moveAction = _playerInputActions.Player.Move;

            moveAction.started += OnMoveChanged;
            moveAction.performed += OnMoveChanged;
            moveAction.canceled += OnMoveChanged;

            _mobileInputStateService.MoveDirection
                .Subscribe(direction =>
                {
                    _mobileMoveDirection = direction;
                    ApplyMoveDirection();
                })
                .AddTo(_disposables);

            _mobileInputStateService.IsMoveStickActive
                .Subscribe(isActive =>
                {
                    _isMobileMoveActive = isActive;
                    ApplyMoveDirection();
                })
                .AddTo(_disposables);

            _actionMoveDirection = moveAction.ReadValue<Vector2>();
            _mobileMoveDirection = _mobileInputStateService.MoveDirection.Value;
            _isMobileMoveActive = _mobileInputStateService.IsMoveStickActive.Value;
            ApplyMoveDirection();
        }

        protected override void OnDisable()
        {
            InputAction moveAction = _playerInputActions.Player.Move;

            moveAction.started -= OnMoveChanged;
            moveAction.performed -= OnMoveChanged;
            moveAction.canceled -= OnMoveChanged;
            _disposables.Clear();

            if (_moveInputData != null)
            {
                _moveInputData.MoveDirection.Value = Vector2.zero;
            }
        }

        private void OnMoveChanged(InputAction.CallbackContext context)
        {
            _inputModeService.ReportDevice(context.control?.device);
            _actionMoveDirection = context.ReadValue<Vector2>();
            ApplyMoveDirection();
        }

        private void ApplyMoveDirection()
        {
            if (_moveInputData == null)
            {
                return;
            }

            _moveInputData.MoveDirection.Value = _isMobileMoveActive
                ? _mobileMoveDirection
                : _actionMoveDirection;
        }
    }
}
