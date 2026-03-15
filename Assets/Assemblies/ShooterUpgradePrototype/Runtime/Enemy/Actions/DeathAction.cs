using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Nody.Runtime.Core;
using OdinSerializer;
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
    [Name("AutoStrike/Actions/Death")]
    [Group("Death")]
    public sealed class DeathAction : EntityMonoBehaviourAction
    {
        private const string HealthId = "HP";

        [OdinSerialize]
        private EntityActionCollection _deathActionStack = new();

        private CompositeDisposable _subscriptions = new();

        [Inject]
        private EnemyRegistryService _registry;

        [Inject]
        private EnemyRewardService _rewardService;

        protected override void SetupComponent(object[] setupData = null)
        {
            _deathActionStack ??= new EntityActionCollection();
            _deathActionStack.SetAllowedGroupAttributes(new[] { "Death" });
            _deathActionStack.Setup(true, setupData);
        }

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
                .Subscribe(_ => HandleDeath().Forget())
                .AddTo(_subscriptions);
        }

        protected override void OnDisable() => _subscriptions?.Clear();

        protected override void OnDisableElement()
        {
            _deathActionStack?.OnDisable();
        }

        private async UniTaskVoid HandleDeath()
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
            _rewardService.Grant(runtimeData.KillRewardPoints.Value);

            CancellationToken cancellationToken = EntityMonoBehaviour.GetCancellationTokenOnDestroy();
            await _deathActionStack.Run(cancellationToken);

            Object.Destroy(enemy.gameObject);
        }
    }
}
