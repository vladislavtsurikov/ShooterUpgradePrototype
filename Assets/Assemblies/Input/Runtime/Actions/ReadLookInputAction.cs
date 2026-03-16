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
    [RequiresData(typeof(LookInputData))]
    [Name("AutoStrike.Input/Actions/ReadLookInput")]
    public sealed class ReadLookInputAction : EntityMonoBehaviourAction
    {
        [Inject]
        private PlayerInputActions _playerInputActions;

        [Inject]
        private MobileInputStateService _mobileInputStateService;

        [Inject]
        private InputModeService _inputModeService;

        private LookInputData _lookInputData;

        protected override void OnEnable()
        {
            _lookInputData = Entity.GetData<LookInputData>();
            _lookInputData.LookDelta.Value = Vector2.zero;
            _lookInputData.LookRate.Value = Vector2.zero;
        }

        protected override void Update()
        {
            InputAction lookAction = _playerInputActions.Player.Look;
            Vector2 look = lookAction.ReadValue<Vector2>();
            if (look.sqrMagnitude > 0.0001f)
            {
                _inputModeService.ReportDevice(lookAction.activeControl.device);
            }

            if ((_mobileInputStateService.IsMoveStickActive.Value || _mobileInputStateService.IsFireButtonPressed.Value)
                && lookAction.activeControl.device is Touchscreen)
            {
                _lookInputData.LookDelta.Value = Vector2.zero;
                _lookInputData.LookRate.Value = Vector2.zero;
                return;
            }

            if (lookAction.activeControl.device is Gamepad)
            {
                _lookInputData.LookRate.Value = look;
                _lookInputData.LookDelta.Value = Vector2.zero;
                return;
            }

            _lookInputData.LookDelta.Value = look;
            _lookInputData.LookRate.Value = Vector2.zero;
        }
    }
}
