using AutoStrike.Input.Data;
using AutoStrike.Input.Generated;
using AutoStrike.Input.Services;
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

        private FireInputData _fireInputData;
        private bool _actionFirePressed;

        protected override void OnEnable()
        {
            _fireInputData = Entity.GetData<FireInputData>();
            InputAction fireAction = _playerInputActions.Player.Fire;

            fireAction.started += OnFireChanged;
            fireAction.performed += OnFireChanged;
            fireAction.canceled += OnFireChanged;

            _actionFirePressed = fireAction.IsPressed();
            _fireInputData.IsFirePressed.Value = _actionFirePressed;
        }

        protected override void OnDisable()
        {
            InputAction fireAction = _playerInputActions.Player.Fire;

            fireAction.started -= OnFireChanged;
            fireAction.performed -= OnFireChanged;
            fireAction.canceled -= OnFireChanged;
            _fireInputData.IsFirePressed.Value = false;
        }

        private void OnFireChanged(InputAction.CallbackContext context)
        {
            _inputModeService.ReportDevice(context.control.device);
            _actionFirePressed = context.ReadValueAsButton();
            _fireInputData.IsFirePressed.Value = _actionFirePressed;
        }
    }
}
