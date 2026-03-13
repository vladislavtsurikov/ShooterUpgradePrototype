using AutoStrike.Data;
using UniRx;
using UnityEngine;
using ShooterUpgradePrototype.Enemy.Entities;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(StatsEntityData), typeof(AttackDistanceData), typeof(TargetData))]
    [Name("AutoStrike/Actions/AttackTarget")]
    public sealed class AttackTargetAction : EntityMonoBehaviourAction
    {
        private const string AttackId = "ATK";
        private const string AttackSpeedId = "ATKSPD";

        private CompositeDisposable _subscriptions = new();
        private float _cooldownRemaining;

        private TargetData _targetData;
        private StatsEntityData _attackerStats;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            _targetData = Entity.GetData<TargetData>();
            _attackerStats = Entity.GetData<StatsEntityData>();

            _targetData.Target
                .Select(target =>
                    target != null
                        ? Observable.EveryUpdate()
                        : Observable.Empty<long>())
                .Switch()
                .Subscribe(_ => AttackStep())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable() => _subscriptions?.Clear();

        private void AttackStep()
        {
            EntityMonoBehaviour target = _targetData.Target.Value;
            if (target == null)
            {
                return;
            }

            float attack = _attackerStats.Stat(AttackId).RuntimeData<RuntimeStatValueData>().CurrentValue;
            float attackCooldown = Mathf.Max(0.01f, _attackerStats.Stat(AttackSpeedId).RuntimeData<RuntimeStatValueData>().CurrentValue);

            _cooldownRemaining -= Time.deltaTime;
            if (_cooldownRemaining > 0f)
            {
                return;
            }

            _cooldownRemaining = attackCooldown;

            EnemyEntity enemy = target.GetComponent<EnemyEntity>() ?? target.GetComponentInParent<EnemyEntity>();
            if (enemy == null)
            {
                return;
            }

            enemy.TryApplyDamage(attack);
        }
    }
}
