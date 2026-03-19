using System;
using Stats.EntityDataActionIntegration;
using Stats.Runtime;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using WaypointsSystem.Runtime;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    public sealed class EnemyEntity : EntityMonoBehaviour
    {
        [SerializeField] private StatCollection _statsCollection;

        protected override Type[] ComponentDataTypesToCreate() =>
            new[]
            {
                typeof(StatsEntityData),
                typeof(WaypointPathData),
                typeof(WaypointPathDirectionData),
            };

        protected override Type[] ActionTypesToCreate() =>
            new[]
            {
                typeof(ModifyStatRandomAction),
                typeof(PatrolMoveAction),
                typeof(PatrolRotateAction),
                typeof(DeathAction)
            };
    }
}
