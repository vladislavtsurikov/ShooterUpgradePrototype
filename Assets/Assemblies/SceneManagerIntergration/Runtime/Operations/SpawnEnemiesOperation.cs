using System.Threading;
using AutoStrike.Spawning.WaypointPathEntitySpawner.Runtime;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using Zenject;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;
using Single = VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem.Single;

namespace ArmyClash.SceneManager.SceneManagerIntergration.Runtime.Operations
{
    [Name("AutoStrike/Spawn Enemies")]
    [ParentRequired(typeof(AfterLoadOperationsSettings), typeof(Single))]
    public sealed class SpawnEnemiesOperation : Action
    {
        [Inject]
        private EnemySpawnConfig _config;

        [Inject]
        private EnemyRegistryService _registry;

        [Inject]
        private WaypointPathEntitySpawner _spawner;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (_config.MaxMobCount > 0)
            {
                _spawner.SpawnMaxCountEntity(_registry.Enemies.Count, _config.MaxMobCount);
            }

            return UniTask.FromResult(true);
        }
    }
}
