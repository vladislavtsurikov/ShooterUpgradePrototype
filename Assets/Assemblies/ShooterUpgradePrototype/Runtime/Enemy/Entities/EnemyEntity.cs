using System;
using AutoStrike.Actions;
using ArmyClash.WaypointsSystem.Runtime.Movement;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;

namespace ShooterUpgradePrototype.Enemy.Entities
{
    public sealed class EnemyEntity : EntityMonoBehaviour
    {
        private const string HealthId = "HP";
        [SerializeField] private StatCollection _statsCollection;

        protected override Type[] ComponentDataTypesToCreate() =>
            new[]
            {
                typeof(StatsEntityData),
                typeof(EnemyRuntimeData),
                typeof(WaypointPathData),
                typeof(WaypointPathDirectionData),
            };

        protected override Type[] ActionTypesToCreate() =>
            new[]
            {
                typeof(TakeDamageAction),
                typeof(PatrolMoveAction),
                typeof(PatrolRotateAction),
                typeof(DeathAction)
            };

        protected override void OnAfterCreateDataAndActions()
        {
            base.OnAfterCreateDataAndActions();

            StatsEntityData stats = GetData<StatsEntityData>();
            if (stats == null)
            {
                return;
            }

            stats.SourceType = StatsEntitySourceType.Local;
            stats.GlobalConfig = null;
            stats.Collection = _statsCollection;
            stats.RebuildFromCollection();
        }

        public bool IsAlive
        {
            get
            {
                EnemyRuntimeData runtimeData = GetData<EnemyRuntimeData>();
                if (runtimeData.IsDead.Value)
                {
                    return false;
                }

                return GetHealthData().CurrentValue > 0f;
            }
        }

        public float CurrentHealth => GetHealthData().CurrentValue;

        public void InitializeSpawn(float startingHealth, int killRewardPoints)
        {
            Setup();

            RuntimeStatValueData healthData = GetHealthData();
            healthData.SetValue(Mathf.Max(0f, startingHealth));

            EnemyRuntimeData runtimeData = GetData<EnemyRuntimeData>();
            runtimeData.Initialize(healthData.CurrentValue, killRewardPoints);
        }

        public bool TryApplyDamage(float damage)
        {
            TakeDamageAction damageAction = GetAction<TakeDamageAction>();
            return damageAction != null && damageAction.TryApplyDamage(damage);
        }

        private RuntimeStatValueData GetHealthData()
        {
            StatsEntityData stats = GetData<StatsEntityData>();
            return stats.Stat(HealthId).RuntimeData<RuntimeStatValueData>();
        }
    }
}
