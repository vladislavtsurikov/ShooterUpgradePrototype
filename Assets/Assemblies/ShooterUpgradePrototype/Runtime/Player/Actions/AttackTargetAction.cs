using AutoStrike.Input.Data;
using AutoStrike.FirstPersonCamera.Data;
using ShooterUpgradePrototype.Enemy.Entities;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(StatsEntityData), typeof(FireInputData), typeof(FirstPersonCameraRigData))]
    [Name("AutoStrike/Actions/AttackTarget")]
    public sealed class AttackTargetAction : EntityMonoBehaviourAction
    {
        private const string AttackId = "ATK";

        [SerializeField]
        [Min(0f)]
        private float _shotCooldown = 0.2f;

        [SerializeField]
        private LayerMask _hitMask = ~0;

        [SerializeField]
        [Min(1f)]
        private float _maxShootDistance = 250f;

        private readonly CompositeDisposable _subscriptions = new();
        private FirstPersonCameraRigData _cameraRigData;
        private StatsEntityData _attackerStats;
        private FireInputData _fireInputData;
        private float _cooldownRemaining;

        protected override void OnEnable()
        {
            _attackerStats = Entity.GetData<StatsEntityData>();
            _fireInputData = Entity.GetData<FireInputData>();
            _cameraRigData = Entity.GetData<FirstPersonCameraRigData>();
            _cooldownRemaining = 0f;

            _subscriptions.Clear();
            _fireInputData.IsFirePressed
                .Select(isPressed =>
                    isPressed
                        ? Observable.EveryUpdate()
                        : Observable.Empty<long>())
                .Switch()
                .Subscribe(_ => AttackStep())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable()
        {
            _subscriptions.Clear();
        }

        protected override void Update()
        {
            if (_cooldownRemaining > 0f)
            {
                _cooldownRemaining = Mathf.Max(0f, _cooldownRemaining - Time.deltaTime);
            }
        }

        private void AttackStep()
        {
            if (_cooldownRemaining > 0f)
            {
                return;
            }

            Camera camera = _cameraRigData?.Camera;
            if (camera == null)
            {
                return;
            }

            float attack = _attackerStats.Stat(AttackId).RuntimeData<RuntimeStatValueData>().CurrentValue;
            _cooldownRemaining = Mathf.Max(0f, _shotCooldown);

            Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (!Physics.Raycast(ray, out RaycastHit hit, _maxShootDistance, _hitMask, QueryTriggerInteraction.Ignore))
            {
                return;
            }

            EnemyEntity enemy = hit.collider.GetComponentInParent<EnemyEntity>();
            if (enemy == null)
            {
                return;
            }

            enemy.TryApplyDamage(attack);
        }
    }
}
