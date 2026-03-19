using AutoStrike.Spawning.WaypointPathEntitySpawner.Runtime;
using ShooterUpgradePrototype.Runtime;
using UnityEngine;
using WaypointsSystem.Runtime;
using Zenject;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
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
            enemy.Setup();

            WaypointPathData data = enemy.GetData<WaypointPathData>();
            data.Path = path;
            _registry.Register(enemy);
        }
    }
}
