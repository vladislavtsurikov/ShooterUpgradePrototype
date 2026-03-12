using AutoStrike.Data;
using AutoStrike.Input.Data;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(TargetData), typeof(AttackDistanceData), typeof(MoveInputData))]
    [Name("AutoStrike/Actions/UpdateAutoAttackTarget")]
    public sealed class UpdateAutoAttackTargetAction : EntityMonoBehaviourAction
    {
        private const int ResultsCapacity = 64;
        private const string HealthId = "HP";

        [SerializeField]
        private LayerMask _enemyLayer = ~0;

        private Collider[] _results;
        private CompositeDisposable _subscriptions = new();
        private TargetData _targetData;
        private AttackDistanceData _attackDistanceData;
        private MoveInputData _inputData;

        protected override void OnEnable()
        {
            _results = new Collider[ResultsCapacity];

            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            _targetData = Entity.GetData<TargetData>();
            _attackDistanceData = Entity.GetData<AttackDistanceData>();
            _inputData = Entity.GetData<MoveInputData>();

            _inputData.MoveDirection
                .Select(direction => direction.sqrMagnitude > 0.0001f)
                .DistinctUntilChanged()
                .Where(isMoving => isMoving)
                .Subscribe(_ => _targetData.Target.Value = null)
                .AddTo(_subscriptions);

            _inputData.MoveDirection
                .Select(direction => direction.sqrMagnitude > 0.0001f)
                .Select(isMoving =>
                    !isMoving
                        ? Observable.EveryUpdate()
                        : Observable.Empty<long>())
                .Switch()
                .Subscribe(_ => FindNearestTargetInRange())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable() => _subscriptions?.Clear();

        protected override void OnDrawGizmos()
        {
            if (EntityMonoBehaviour == null)
            {
                return;
            }

            _attackDistanceData ??= Entity.GetData<AttackDistanceData>();

            Vector3 selfPosition = EntityMonoBehaviour.transform.position;
            float attackRange = Mathf.Max(0f, _attackDistanceData.AttackRange.Value);
            int hitCount = Physics.OverlapSphereNonAlloc(
                selfPosition,
                attackRange,
                _results,
                _enemyLayer,
                QueryTriggerInteraction.Ignore);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(selfPosition, attackRange);

            Vector3 closestPosition = Vector3.zero;
            float minDistanceSq = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                Collider collider = _results[i];
                if (collider == null)
                {
                    continue;
                }

                EntityMonoBehaviour candidate = collider.GetComponentInParent<EntityMonoBehaviour>();
                Vector3 hitPosition = candidate != null
                    ? candidate.transform.position
                    : collider.bounds.center;

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(selfPosition, hitPosition);
                Gizmos.DrawWireSphere(hitPosition, 0.12f);

                float sqrDistance = (hitPosition - selfPosition).sqrMagnitude;
                if (sqrDistance >= minDistanceSq)
                {
                    continue;
                }

                minDistanceSq = sqrDistance;
                closestPosition = hitPosition;
            }

            if (hitCount > 0 && minDistanceSq < float.MaxValue)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(selfPosition, closestPosition);
                Gizmos.DrawWireSphere(closestPosition, 0.18f);
            }
        }

        private void FindNearestTargetInRange()
        {
            float attackRange = _attackDistanceData.AttackRange.Value;

            Vector3 selfPosition = EntityMonoBehaviour.transform.position;
            int hitCount = Physics.OverlapSphereNonAlloc(
                selfPosition,
                attackRange,
                _results,
                _enemyLayer,
                QueryTriggerInteraction.Ignore);

            EntityMonoBehaviour closest = null;
            float minDistanceSq = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                Collider collider = _results[i];
                if (collider == null)
                {
                    continue;
                }

                EntityMonoBehaviour candidate = collider.GetComponentInParent<EntityMonoBehaviour>();
                if (!IsTargetValid(candidate))
                {
                    continue;
                }

                Vector3 candidatePosition = candidate.transform.position;
                float sqrDistance = (candidatePosition - selfPosition).sqrMagnitude;
                if (sqrDistance > attackRange * attackRange)
                {
                    continue;
                }

                if (sqrDistance >= minDistanceSq)
                {
                    continue;
                }

                minDistanceSq = sqrDistance;
                closest = candidate;
            }

            _targetData.Target.Value = closest;
        }

        private bool IsTargetValid(EntityMonoBehaviour candidate)
        {
            if (!IsAlive(candidate))
            {
                return false;
            }

            return IsInRange(candidate);
        }

        private bool IsInRange(EntityMonoBehaviour candidate)
        {
            float attackRange = Mathf.Max(0f, _attackDistanceData.AttackRange.Value);
            float sqrDistance = (candidate.transform.position - EntityMonoBehaviour.transform.position).sqrMagnitude;
            return sqrDistance <= attackRange * attackRange;
        }

        private static bool IsAlive(EntityMonoBehaviour candidate)
        {
            StatsEntityData stats = candidate.GetData<StatsEntityData>();
            return stats.GetRuntimeStatById(HealthId).Runtime().Data<RuntimeStatValueData>().CurrentValue > 0f;
        }
    }
}
