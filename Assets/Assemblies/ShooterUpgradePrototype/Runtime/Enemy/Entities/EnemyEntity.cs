using System;
using System.Linq;
using AutoStrike.Actions;
using ArmyClash.WaypointsSystem.Runtime.Movement;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;

namespace ShooterUpgradePrototype.Enemy.Entities
{
    public sealed class EnemyEntity : EntityMonoBehaviour
    {
        [SerializeField] private StatCollection _statsCollection;

        protected override Type[] ComponentDataTypesToCreate() =>
            new[]
            {
                typeof(StatsEntityData),
                typeof(EnemyRuntimeData),
                typeof(WaypointPathData),
                typeof(WaypointPathDirectionData),
            };

        protected override Type[] ActionTypesToCreate() =>
            new[]
            {
                typeof(ModifyStatRandomAction),
                typeof(TakeDamageAction),
                typeof(PatrolMoveAction),
                typeof(PatrolRotateAction),
                typeof(DeathAction)
            };
    }
}
