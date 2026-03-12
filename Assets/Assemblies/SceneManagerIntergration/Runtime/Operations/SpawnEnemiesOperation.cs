using System.Threading;
using AutoStrike.Spawning;
using Cysharp.Threading.Tasks;
using ShooterUpgradePrototype.Enemy.Services;
using ShooterUpgradePrototype.Progression.Configs;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using Zenject;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;
using Single = VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem.Single;

namespace ArmyClash.SceneManager
{
    [Name("AutoStrike/Spawn Enemies")]
    [ParentRequired(typeof(AfterLoadOperationsSettings), typeof(Single))]
    public sealed class SpawnEnemiesOperation : Action
    {
        [Inject]
        private ShooterUpgradePrototypeConfig _config;

        [Inject]
        private EnemyRegistryService _registry;

        [Inject]
        private WaypointPathEntitySpawner _spawner;

        protected override UniTask<bool> Run(CancellationToken token)
        {
            if (_config.Enemy.MaxMobCount > 0)
            {
                _spawner.SpawnMaxCountEntity(_registry.Enemies.Count, _config.Enemy.MaxMobCount);
            }

            return UniTask.FromResult(true);
        }
    }
}
