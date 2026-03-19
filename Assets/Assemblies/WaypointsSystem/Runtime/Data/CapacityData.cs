using Nody.Runtime.Core;
using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace WaypointsSystem.Runtime
{
    [Persistent]
    [Name("Waypoints/Spawning/Capacity Data")]
    [Group("Waypoints")]
    public sealed class CapacityData : ComponentData
    {
        public enum CapacityMode
        {
            Unlimited = 0,
            Limited = 1
        }

        [OdinSerialize]
        [Min(1)]
        private int _capacity = 1;

        [OdinSerialize]
        public CapacityMode Mode = CapacityMode.Unlimited;

        public int Capacity
        {
            get => _capacity;
            set => _capacity = Mathf.Max(1, value);
        }
    }
}
