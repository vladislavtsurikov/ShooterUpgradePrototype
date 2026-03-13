using System;
using AutoStrike.Actions;
using AutoStrike.Data;
using AutoStrike.Input.Actions;
using AutoStrike.Input.Data;
using ArmyClash.Battle.Data;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;

namespace AutoStrike.Entities
{
    public sealed class PlayerEntity : EntityMonoBehaviour
    {
        protected override Type[] ComponentDataTypesToCreate() =>
            new[]
            {
                typeof(StatsEntityData),
                typeof(ModifiersData),
                typeof(TargetData),
                typeof(AttackDistanceData),
                typeof(MoveInputData),
                typeof(LookInputData),
                typeof(FireInputData),
            };

        protected override Type[] ActionTypesToCreate() =>
            new[]
            {
                typeof(ApplyModifierStatEffectAction),
                typeof(ApplyStatLevelsByTableAction),
                typeof(ReadMoveInputAction),
                typeof(ReadLookInputAction),
                typeof(ReadFireInputAction),
                typeof(MoveByInputAction),
                typeof(UpdateAutoAttackTargetAction),
                typeof(RotateByInputAction),
                typeof(RotateToTargetAction),
                typeof(AttackTargetAction)
            };
    }
}
