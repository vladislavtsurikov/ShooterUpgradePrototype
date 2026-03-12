using AutoStrike.Input.Data;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.FirstPersonCamera.Actions
{
    [RequiresData(typeof(LookInputData))]
    [Name("AutoStrike.FirstPersonCamera/Actions/MobileFirstPersonCameraLook")]
    public sealed class MobileFirstPersonCameraLookAction : FirstPersonCameraLookActionBase
    {
        [Header("Touch Drag")]
        [SerializeField]
        private Vector2 _dragSensitivity = new(0.12f, 0.12f);

        [Header("Virtual Right Stick")]
        [SerializeField]
        private Vector2 _rateSensitivity = new(180f, 180f);

        [SerializeField]
        private bool _useUnscaledDeltaTime;

        private void Update()
        {
            Vector2 look = Vector2.Scale(LookInputData.LookDelta.Value, _dragSensitivity);

            float deltaTime = _useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;
            look += Vector2.Scale(LookInputData.LookRate.Value, _rateSensitivity) * deltaTime;

            ApplyLook(look);
        }
    }
}
