using AutoStrike.Input.Generated;
using AutoStrike.Input.InputMode.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using Zenject;

namespace AutoStrike.Input.FPSInput.Runtime
{
    [RequiresData(typeof(LookInputData))]
    [Name("AutoStrike.Input/Actions/ReadLookInput")]
    public sealed class ReadLookInputAction : EntityMonoBehaviourAction
    {
        [Inject]
        private PlayerInputActions _playerInputActions;

        [Inject]
        private InputModeService _inputModeService;

        private LookInputData _lookInputData;

        protected override void OnEnable()
        {
            _lookInputData = Entity.GetData<LookInputData>();
            ResetLook();
        }

        protected override void Update()
        {
            InputAction lookAction = _playerInputActions.Player.Look;
            Vector2 look = lookAction.ReadValue<Vector2>();

            if (look.sqrMagnitude > 0.0001f)
            {
                _inputModeService.ReportDevice(lookAction.activeControl?.device);
            }

            ApplyLook(lookAction, look);
        }

        private void ApplyLook(InputAction lookAction, Vector2 look)
        {
            InputDevice device = lookAction.activeControl?.device;

            if (device is Gamepad)
            {
                _lookInputData.LookRate.Value = look;
                _lookInputData.LookDelta.Value = Vector2.zero;
                return;
            }

            _lookInputData.LookDelta.Value = look;
            _lookInputData.LookRate.Value = Vector2.zero;
        }

        private void ResetLook()
        {
            _lookInputData.LookDelta.Value = Vector2.zero;
            _lookInputData.LookRate.Value = Vector2.zero;
        }
    }
}
