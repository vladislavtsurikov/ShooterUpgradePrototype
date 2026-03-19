using ShooterUpgradePrototype.Runtime;
using Stats.EntityDataActionIntegration;
using UnityEngine;
using Zenject;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
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
