using ArmyClash.WaypointsSystem.Runtime;
using ArmyClash.WaypointsSystem.Runtime.Movement;
using ShooterUpgradePrototype.Enemy.Entities;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(WaypointPathData), typeof(WaypointPathDirectionData), typeof(EnemyRuntimeData))]
    [Name("AutoStrike/Actions/PatrolRotate")]
    public sealed class PatrolRotateAction : EntityMonoBehaviourAction
    {
        [SerializeField]
        private float _interpolationSpeed = 12f;

        private WaypointPathData _waypointPathData;
        private WaypointPathDirectionData _directionData;
        private EnemyRuntimeData _runtimeData;

        protected override void OnEnable()
        {
            _waypointPathData = Entity.GetData<WaypointPathData>();
            _directionData = Entity.GetData<WaypointPathDirectionData>();
            _runtimeData = Entity.GetData<EnemyRuntimeData>();
        }

        protected override void Update()
        {
            if (_runtimeData.IsDead.Value)
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
