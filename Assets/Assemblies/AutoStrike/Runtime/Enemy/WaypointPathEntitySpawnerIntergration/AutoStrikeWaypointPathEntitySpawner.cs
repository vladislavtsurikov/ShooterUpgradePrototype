using ArmyClash.WaypointsSystem.Runtime;
using ArmyClash.WaypointsSystem.Runtime.Movement;
using AutoStrike.Entities;
using AutoStrike.Services;
using UnityEngine;
using Zenject;

namespace AutoStrike.Spawning
{
    public sealed class AutoStrikeWaypointPathEntitySpawner : WaypointPathEntitySpawner
    {
        [Inject]
        private EnemyRegistryService _registry;

        protected override void OnAfterSpawn(GameObject instance, WaypointPath path, float normalizedPosition)
        {
            EnemyEntity enemy = instance.GetComponent<EnemyEntity>();

            WaypointPathData data = enemy.GetData<WaypointPathData>();
            data.Path = path;

            _registry.Register(enemy);
        }
    }
}
