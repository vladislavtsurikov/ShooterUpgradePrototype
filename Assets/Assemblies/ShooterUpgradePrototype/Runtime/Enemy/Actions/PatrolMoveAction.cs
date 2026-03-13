using ArmyClash.WaypointsSystem.Runtime;
using ArmyClash.WaypointsSystem.Runtime.Movement;
using ArmyClash.WaypointsSystem.Runtime.Spawning;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.StateMachine.Runtime.Data;
using UnityEngine;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(WaypointPathData), typeof(StatsEntityData))]
    [Name("AutoStrike/Actions/PatrolMove")]
    public sealed class PatrolMoveAction : EntityMonoBehaviourAction
    {
        private const string SpeedId = "SPEED";

        private WaypointPathData _data;
        private StatsEntityData _stats;

        protected override void OnEnable()
        {
            _data = Entity.GetData<WaypointPathData>();
            _stats = Entity.GetData<StatsEntityData>();
        }

        protected override void FixedUpdate()
        {
            float speed = _stats.Stat(SpeedId).RuntimeData<RuntimeStatValueData>().CurrentValue;
            WaypointPath path = _data.Path;
            float pathLength = path.GetData<WaypointPathMetricsData>().GetTotalLength();
            float deltaNormalized = speed * Time.fixedDeltaTime / pathLength;
            path.TryMoveAlongPath(EntityMonoBehaviour, deltaNormalized);
        }
    }
}
