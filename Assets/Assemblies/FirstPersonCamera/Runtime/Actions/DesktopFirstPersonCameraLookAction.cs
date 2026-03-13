using AutoStrike.Input.Data;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.FirstPersonCamera.Actions
{
    [RequiresData(typeof(LookInputData))]
    [Name("AutoStrike.FirstPersonCamera/Actions/DesktopFirstPersonCameraLook")]
    public sealed class DesktopFirstPersonCameraLookAction : FirstPersonCameraLookAction
    {
        [SerializeField]
        private Vector2 _sensitivity = new(0.15f, 0.15f);

        protected override void HandleLookDelta(Vector2 lookDelta)
        {
            Vector2 look = Vector2.Scale(lookDelta, _sensitivity);
            ApplyLook(look);
        }
    }
}
