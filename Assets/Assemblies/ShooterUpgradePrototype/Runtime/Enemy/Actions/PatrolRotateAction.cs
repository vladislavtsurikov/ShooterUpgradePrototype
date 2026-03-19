using Stats.EntityDataActionIntegration;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using WaypointsSystem.Runtime;

namespace ShooterUpgradePrototype.Runtime
{
    [RequiresData(typeof(WaypointPathData), typeof(WaypointPathDirectionData), typeof(StatsEntityData))]
    [Name("AutoStrike/Actions/PatrolRotate")]
    public sealed class PatrolRotateAction : EntityMonoBehaviourAction
    {
        private const string HealthId = "HP";

        [SerializeField]
        private float _interpolationSpeed = 12f;

        private StatsEntityData _stats;
        private WaypointPathData _waypointPathData;
        private WaypointPathDirectionData _directionData;

        protected override void OnEnable()
        {
            _stats = Entity.GetData<StatsEntityData>();
            _waypointPathData = Entity.GetData<WaypointPathData>();
            _directionData = Entity.GetData<WaypointPathDirectionData>();
        }

        protected override void Update()
        {
            float health = _stats.Stat(HealthId).RuntimeData<RuntimeStatValueData>().CurrentValue;
            if (health <= 0f || _waypointPathData.Path == null)
            {
                return;
            }

            if (!WaypointPathNormalizedPositionUtility.TryGetSegmentPointsByWorldPosition(
                    _waypointPathData.Path,
                    EntityMonoBehaviour.transform.position,
                    out Vector3 from,
                    out Vector3 targetPoint))
            {
                return;
            }

            if (_directionData.Direction < 0)
            {
                (from, targetPoint) = (targetPoint, from);
            }

            Transform transform = EntityMonoBehaviour.transform;
            from.y = transform.position.y;
            targetPoint.y = transform.position.y;

            Vector3 forward = targetPoint - from;
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(forward.normalized, Vector3.up);
            float speed = Mathf.Max(0f, _interpolationSpeed);
            float t = 1f - Mathf.Exp(-speed * Time.deltaTime);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, t);
        }
    }
}
