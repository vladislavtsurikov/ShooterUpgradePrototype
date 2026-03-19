using UnityEngine;
using Zenject;

namespace AutoStrike.Spawning.WaypointPathEntitySpawner.Runtime
{
    [AddComponentMenu("AutoStrike/Spawning/Entity Spawner Installer")]
    public sealed class WaypointPathEntitySpawnerInstaller : MonoInstaller
    {
        [SerializeField]
        private WaypointPathEntitySpawner _spawner;

        public override void InstallBindings()
        {
            Container.Bind<WaypointPathEntitySpawner>().FromInstance(_spawner).AsSingle();
        }
    }
}
