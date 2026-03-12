using UnityEngine;
using Zenject;

namespace ArmyClash.WaypointsSystem.Runtime.Installers
{
    [AddComponentMenu("ArmyClash/Waypoints/Waypoints System Installer")]
    public sealed class WaypointsSystemInstaller : MonoInstaller
    {
        [SerializeField]
        private WaypointsSystem _system;

        public override void InstallBindings()
        {
            Container.Bind<WaypointsSystem>().FromInstance(_system).AsSingle();
        }
    }
}
