using ShooterUpgradePrototype.Player.Services;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using Zenject;

namespace ShooterUpgradePrototype.Infrastructure.Installers
{
    public sealed class PlayerStatsInstaller : MonoInstaller
    {
        [SerializeField] private StatsEntityConfig _playerStatsConfig;

        public override void InstallBindings()
        {
            Container.Bind<StatsEntityConfig>().FromInstance(_playerStatsConfig).AsSingle();
            Container.Bind<PlayerStatsService>().AsSingle();
        }
    }
}
