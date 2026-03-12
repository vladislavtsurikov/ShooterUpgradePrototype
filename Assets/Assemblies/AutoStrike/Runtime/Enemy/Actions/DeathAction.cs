using AutoStrike.Entities;
using AutoStrike.Services;
using UniRx;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;
using Zenject;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(StatsEntityData))]
    [Name("AutoStrike/Actions/EnemyDeath")]
    public sealed class DeathAction : EntityMonoBehaviourAction
    {
        private const string HealthId = "HP";
        private CompositeDisposable _subscriptions = new();

        [Inject]
        private EnemyRegistryService _registry;

        [Inject]
        private KillCounterService _killCounter;

        protected override void OnEnable()
        {
            _subscriptions ??= new CompositeDisposable();
            _subscriptions.Clear();

            StatsEntityData stats = Entity.GetData<StatsEntityData>();
            RuntimeStat health = stats.GetRuntimeStatById(HealthId);
            health.Runtime().Data<RuntimeStatValueData>().Value
                .Where(currentValue => currentValue == 0f)
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

            EnemyEntity enemy = (EnemyEntity)EntityMonoBehaviour;
            _registry.Unregister(enemy);
            _killCounter.Increment();

            Object.Destroy(EntityMonoBehaviour.gameObject);
        }
    }
}
