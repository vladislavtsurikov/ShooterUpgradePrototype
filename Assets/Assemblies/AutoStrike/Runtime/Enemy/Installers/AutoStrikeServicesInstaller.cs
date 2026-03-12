using AutoStrike.Services;
using ShooterUpgradePrototype.Progression.Services;
using Zenject;

namespace AutoStrike.Installers
{
    public sealed class AutoStrikeServicesInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<KillCounterService>().AsSingle();
            Container.Bind<EnemyRegistryService>().AsSingle();
            Container.Bind<PlayerUpgradeService>().AsSingle();

            Container.BindInterfacesAndSelfTo<AutoRespawnService>().AsSingle().NonLazy();
        }
    }
}
