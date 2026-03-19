using UnityEngine;
using Zenject;

namespace WaypointsSystem.Runtime
{
    [AddComponentMenu("ArmyClash/Waypoints/Waypoints System Installer")]
    public sealed class WaypointsSystemInstaller : MonoInstaller
    {
        [SerializeField]
        private global::WaypointsSystem.Runtime.WaypointsSystem _system;

        public override void InstallBindings()
        {
            Container.Bind<global::WaypointsSystem.Runtime.WaypointsSystem>().FromInstance(_system).AsSingle();
        }
    }
}
