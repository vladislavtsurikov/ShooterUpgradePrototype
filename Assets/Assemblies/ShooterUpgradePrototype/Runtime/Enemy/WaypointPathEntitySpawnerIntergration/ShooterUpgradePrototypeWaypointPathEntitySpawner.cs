using AutoStrike.Config;
using AutoStrike.Spawning;
using ArmyClash.WaypointsSystem.Runtime;
using ArmyClash.WaypointsSystem.Runtime.Movement;
using ShooterUpgradePrototype.Enemy.Entities;
using ShooterUpgradePrototype.Enemy.Services;
using UnityEngine;
using Zenject;

namespace ShooterUpgradePrototype.Spawning
{
    public sealed class ShooterUpgradePrototypeWaypointPathEntitySpawner : WaypointPathEntitySpawner
    {
        [Inject]
        private EnemyRegistryService _registry;

        [Inject]
        private EnemySpawnConfig _config;

        protected override void OnAfterSpawn(GameObject instance, WaypointPath path, float normalizedPosition)
        {
            EnemyEntity enemy = instance.GetComponent<EnemyEntity>();
            if (enemy == null)
            {
                return;
            }

            enemy.Setup();

            WaypointPathData data = enemy.GetData<WaypointPathData>();
            data.Path = path;

            enemy.InitializeSpawn(_config.GetRandomStartingHealth(), _config.KillRewardPoints);
            _registry.Register(enemy);
        }
    }
}
