using ShooterUpgradePrototype.Enemy.Services;
using ShooterUpgradePrototype.Player.Services;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using Zenject;

namespace ShooterUpgradePrototype.Infrastructure.Installers
{
    public sealed class ShooterUpgradePrototypeInstaller : MonoInstaller
    {
        [SerializeField] private StatsEntityConfig _playerStatsConfig;

        public override void InstallBindings()
        {
            Container.Bind<StatsEntityConfig>().FromInstance(_playerStatsConfig).AsSingle();
            Container.Bind<PlayerStatsService>().AsSingle();

            Container.Bind<EnemyRewardService>().AsSingle();
            Container.Bind<EnemyRegistryService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AutoRespawnService>().AsSingle().NonLazy();
        }
    }
}
