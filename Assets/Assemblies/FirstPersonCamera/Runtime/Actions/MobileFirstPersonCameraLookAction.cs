using AutoStrike.Input.Data;
using AutoStrike.Input.Services;
using AutoStrike.Input.Services.States;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.FirstPersonCamera.Actions
{
    [RequiresData(typeof(LookInputData), typeof(Data.CameraData))]
    [Name("AutoStrike.FirstPersonCamera/Actions/MobileFirstPersonCameraLook")]
    public sealed class MobileFirstPersonCameraLookAction : FirstPersonCameraLookAction
    {
        [Header("Touch Drag")]
        [SerializeField]
        private Vector2 _dragSensitivity = new(0.12f, 0.12f);

        [Header("Virtual Right Stick")]
        [SerializeField]
        private Vector2 _rateSensitivity = new(180f, 180f);

        [SerializeField]
        private bool _useUnscaledDeltaTime;

        protected override bool SupportsState(InputModeState state) =>
            state is TouchscreenInputModeState or GamepadInputModeState;

        protected override void HandleLookDelta(Vector2 lookDelta)
        {
            Vector2 look = Vector2.Scale(lookDelta, _dragSensitivity);
            ApplyLook(look);
        }

        protected override void HandleLookRate(Vector2 lookRate)
        {
            float deltaTime = _useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
            Vector2 look = Vector2.Scale(lookRate, _rateSensitivity) * deltaTime;
            ApplyLook(look);
        }
    }
}
