using AutoStrike.Input.Data;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.FirstPersonCamera.Actions
{
    [RequiresData(typeof(LookInputData))]
    [Name("AutoStrike.FirstPersonCamera/Actions/DesktopFirstPersonCameraLook")]
    public sealed class DesktopFirstPersonCameraLookAction : FirstPersonCameraLookActionBase
    {
        [SerializeField]
        private Vector2 _sensitivity = new(0.15f, 0.15f);

        private void Update()
        {
            Vector2 look = Vector2.Scale(LookInputData.LookDelta.Value, _sensitivity);
            ApplyLook(look);
        }
    }
}
