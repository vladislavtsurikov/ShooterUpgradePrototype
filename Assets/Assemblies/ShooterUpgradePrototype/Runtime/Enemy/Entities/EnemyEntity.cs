using System;
using System.Linq;
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
                typeof(ModifyStatRandomAction),
                typeof(TakeDamageAction),
                typeof(PatrolMoveAction),
                typeof(PatrolRotateAction),
                typeof(DeathAction)
            };

        protected override void OnAfterCreateDataAndActions()
        {
            StatsEntityData statsEntityData = GetData<StatsEntityData>();
            statsEntityData.Collection = _statsCollection;
            statsEntityData.RebuildFromCollection();
        }

        public bool IsAlive
        {
            get
            {
                EnemyRuntimeData runtimeData = GetData<EnemyRuntimeData>();
                return runtimeData != null && !runtimeData.IsDead.Value && CurrentHealth > 0f;
            }
        }

        public float CurrentHealth => GetHealthData()?.CurrentValue ?? 0f;

        public void InitializeSpawn(float minHealth, float maxHealth, int killRewardPoints)
        {
            ApplyRandomSpawnHealth(minHealth, maxHealth);

            float spawnedMaxHealth = Mathf.Max(0f, CurrentHealth);
            GetData<EnemyRuntimeData>().Initialize(spawnedMaxHealth, killRewardPoints);
        }

        private RuntimeStatValueData GetHealthData()
        {
            StatsEntityData statsEntityData = GetData<StatsEntityData>();
            return statsEntityData?.Stat(HealthId).RuntimeData<RuntimeStatValueData>();
        }

        private void ApplyRandomSpawnHealth(float minHealth, float maxHealth)
        {
            Stat healthStat = _statsCollection?.Stats?.FirstOrDefault(stat => stat != null && stat.Id == HealthId);
            if (healthStat == null)
            {
                Debug.LogError($"Stat `{HealthId}` was not found in `{name}` stats collection.", this);
                return;
            }

            ModifyStatRandomAction modifyHealthAction = GetAction<ModifyStatRandomAction>();
            modifyHealthAction.Configure(healthStat, minHealth, maxHealth);
            modifyHealthAction.ApplyNow();
        }
    }
}
