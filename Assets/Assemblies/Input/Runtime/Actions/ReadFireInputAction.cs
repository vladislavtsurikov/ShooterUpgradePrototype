using AutoStrike.Input.Data;
using AutoStrike.Input.Generated;
using AutoStrike.Input.Services;
using UniRx;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using UnityEngine.InputSystem;
using Zenject;

namespace AutoStrike.Input.Actions
{
    [RequiresData(typeof(FireInputData))]
    [Name("AutoStrike.Input/Actions/ReadFireInput")]
    public sealed class ReadFireInputAction : EntityMonoBehaviourAction
    {
        [Inject]
        private PlayerInputActions _playerInputActions;

        [Inject]
        private InputModeService _inputModeService;

        [Inject]
        private MobileInputStateService _mobileInputStateService;

        private FireInputData _fireInputData;
        private readonly CompositeDisposable _disposables = new();
        private bool _actionFirePressed;
        private bool _mobileFirePressed;

        protected override void OnEnable()
        {
            _fireInputData = Entity.GetData<FireInputData>();
            InputAction fireAction = _playerInputActions.Player.Fire;

            fireAction.started += OnFireChanged;
            fireAction.performed += OnFireChanged;
            fireAction.canceled += OnFireChanged;

            _mobileInputStateService.IsFireButtonPressed
                .Subscribe(isPressed =>
                {
                    _mobileFirePressed = isPressed;
                    ApplyFirePressed();
                })
                .AddTo(_disposables);

            _actionFirePressed = fireAction.IsPressed();
            _mobileFirePressed = _mobileInputStateService.IsFireButtonPressed.Value;
            ApplyFirePressed();
        }

        protected override void OnDisable()
        {
            InputAction fireAction = _playerInputActions.Player.Fire;

            fireAction.started -= OnFireChanged;
            fireAction.performed -= OnFireChanged;
            fireAction.canceled -= OnFireChanged;
            _disposables.Clear();

            if (_fireInputData != null)
            {
                _fireInputData.IsFirePressed.Value = false;
            }
        }

        private void OnFireChanged(InputAction.CallbackContext context)
        {
            _inputModeService.ReportDevice(context.control?.device);
            _actionFirePressed = context.ReadValueAsButton();
            ApplyFirePressed();
        }

        private void ApplyFirePressed()
        {
            if (_fireInputData == null)
            {
                return;
            }

            _fireInputData.IsFirePressed.Value = _actionFirePressed || _mobileFirePressed;
        }
    }
}
