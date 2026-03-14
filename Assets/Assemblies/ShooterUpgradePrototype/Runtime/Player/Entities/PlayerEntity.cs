using System;
using AutoStrike.Actions;
using AutoStrike.FirstPersonCamera.Actions;
using AutoStrike.FirstPersonCamera.Data;
using AutoStrike.Input.Actions;
using AutoStrike.Input.Data;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace ShooterUpgradePrototype.Player.Entities
{
    public sealed class PlayerEntity : EntityMonoBehaviour
    {
        protected override Type[] ComponentDataTypesToCreate() =>
            new[]
            {
                typeof(StatsEntityData),
                typeof(CameraData),
                typeof(MoveInputData),
                typeof(LookInputData),
                typeof(FireInputData),
            };

        protected override Type[] ActionTypesToCreate() =>
            new[]
            {
                typeof(ApplyStatLevelsByTableAction),
                typeof(ReadMoveInputAction),
                typeof(ReadLookInputAction),
                typeof(ReadFireInputAction),
                typeof(MoveByInputAction),
                typeof(DesktopFirstPersonCameraLookAction),
                typeof(AttackTargetAction)
            };
    }
}
