using AutoStrike.Data;
using UniRx;
using UnityEngine;
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
        private const string HealthId = "HP";

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

            StatsEntityData targetStats = target.GetData<StatsEntityData>();

            float attack = _attackerStats.GetStatValueById(AttackId);
            float attackCooldown = Mathf.Max(0.01f, _attackerStats.GetStatValueById(AttackSpeedId));

            _cooldownRemaining -= Time.deltaTime;
            if (_cooldownRemaining > 0f)
            {
                return;
            }

            _cooldownRemaining = attackCooldown;
            RuntimeStat healthStat = targetStats.GetRuntimeStatById(HealthId);
            RuntimeStatValueData healthData = healthStat.Runtime().Data<RuntimeStatValueData>();
            if (!healthData.AddValue(-attack))
            {
                return;
            }

            targetStats.NotifyStatChanged(HealthId);
        }
    }
}
