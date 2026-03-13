using UniRx;
using UnityEngine;
using ShooterUpgradePrototype.Enemy.Entities;
using ShooterUpgradePrototype.Enemy.Services;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;
using Zenject;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(StatsEntityData), typeof(EnemyRuntimeData))]
    [Name("AutoStrike/Actions/EnemyDeath")]
    public sealed class DeathAction : EntityMonoBehaviourAction
    {
        private const string HealthId = "HP";
        private CompositeDisposable _subscriptions = new();

        [Inject]
        private EnemyRegistryService _registry;

        [Inject]
        private KillCounterService _killCounter;

        [Inject]
        private EnemyRewardService _rewardService;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            StatsEntityData stats = Entity.GetData<StatsEntityData>();
            EnemyRuntimeData runtimeData = Entity.GetData<EnemyRuntimeData>();

            stats.Stat(HealthId).RuntimeData<RuntimeStatValueData>().Value
                .CombineLatest(runtimeData.SpawnedMaxHealth, (currentValue, spawnedMaxHealth) =>
                    spawnedMaxHealth > 0f && currentValue <= 0f)
                .Where(shouldDie => shouldDie)
                .Take(1)
                .Subscribe(_ => HandleDeath())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable() => _subscriptions?.Clear();

        private void HandleDeath()
        {
            if (EntityMonoBehaviour == null)
            {
                return;
            }

            EnemyRuntimeData runtimeData = Entity.GetData<EnemyRuntimeData>();
            if (!runtimeData.TryMarkDead())
            {
                return;
            }

            EnemyEntity enemy = (EnemyEntity)EntityMonoBehaviour;
            _registry.Unregister(enemy);
            _killCounter.Increment();
            _rewardService.Grant(runtimeData.KillRewardPoints.Value);

            Object.Destroy(EntityMonoBehaviour.gameObject);
        }
    }
}
