using ShooterUpgradePrototype.Runtime;
using Zenject;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
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
