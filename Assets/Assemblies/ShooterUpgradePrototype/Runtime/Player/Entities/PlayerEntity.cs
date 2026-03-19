using System;
using AutoStrike.FirstPersonCamera.Data;
using AutoStrike.FirstPersonCamera.FirstPersonCamera.Runtime;
using AutoStrike.Input.FPSInput.Runtime;
using Stats.EntityDataActionIntegration;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
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
                typeof(FirstPersonCameraLookAction),
                typeof(AttackTargetAction),
                typeof(PlayShootAudioAction)
            };
    }
}
