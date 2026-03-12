using AutoStrike.Input.Data;
using AutoStrike.Input.Generated;
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

        private FireInputData _fireInputData;

        protected override void OnEnable()
        {
            _fireInputData = Entity.GetData<FireInputData>();
            InputAction fireAction = _playerInputActions.Player.Fire;

            fireAction.started += OnFireChanged;
            fireAction.performed += OnFireChanged;
            fireAction.canceled += OnFireChanged;

            _fireInputData.IsFirePressed.Value = fireAction.IsPressed();
        }

        protected override void OnDisable()
        {
            InputAction fireAction = _playerInputActions.Player.Fire;

            fireAction.started -= OnFireChanged;
            fireAction.performed -= OnFireChanged;
            fireAction.canceled -= OnFireChanged;

            if (_fireInputData != null)
            {
                _fireInputData.IsFirePressed.Value = false;
            }
        }

        private void OnFireChanged(InputAction.CallbackContext context)
        {
            _fireInputData.IsFirePressed.Value = context.ReadValueAsButton();
        }
    }
}
