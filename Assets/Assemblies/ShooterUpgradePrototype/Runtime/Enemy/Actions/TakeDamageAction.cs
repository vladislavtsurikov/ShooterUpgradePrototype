using ShooterUpgradePrototype.Enemy.Entities;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.ReflectionUtility;

namespace AutoStrike.Actions
{
    [RequiresData(typeof(StatsEntityData), typeof(EnemyRuntimeData))]
    [Name("ShooterUpgradePrototype/Actions/EnemyTakeDamage")]
    public sealed class TakeDamageAction : EntityMonoBehaviourAction
    {
        private const string HealthId = "HP";

        public bool TryApplyDamage(float damage)
        {
            if (damage <= 0f)
            {
                return false;
            }

            EnemyRuntimeData runtimeData = Entity.GetData<EnemyRuntimeData>();
            if (runtimeData.IsDead.Value)
            {
                return false;
            }

            RuntimeStatValueData healthData = Entity.GetData<StatsEntityData>()
                .Stat(HealthId)
                .RuntimeData<RuntimeStatValueData>();

            if (healthData.CurrentValue <= 0f)
            {
                return false;
            }

            return healthData.AddValue(-damage);
        }
    }
}
