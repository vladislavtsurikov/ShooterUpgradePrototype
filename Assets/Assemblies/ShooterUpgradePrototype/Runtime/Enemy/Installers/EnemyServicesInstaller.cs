using Zenject;

namespace ShooterUpgradePrototype.Runtime
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
