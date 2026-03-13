using ShooterUpgradePrototype.Enemy.Services;
using Zenject;

namespace ShooterUpgradePrototype.Enemy.Installers
{
    public sealed class EnemyServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EnemyRewardService>().AsSingle();
            Container.Bind<EnemyRegistryService>().AsSingle();

            Container.BindInterfacesAndSelfTo<AutoRespawnService>().AsSingle().NonLazy();
        }
    }
}
