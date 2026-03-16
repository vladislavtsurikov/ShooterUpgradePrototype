using ShooterUpgradePrototype.Enemy.Services;
using Zenject;

namespace ShooterUpgradePrototype.Infrastructure.Installers
{
    public sealed class ShooterUpgradePrototypeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EnemyRewardService>().AsSingle();
            Container.Bind<EnemyRegistryService>().AsSingle();
            Container.BindInterfacesAndSelfTo<AutoRespawnService>().AsSingle().NonLazy();
        }
    }
}
