using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace WaypointsSystem.Runtime
{
    [Persistent]
    [Name("Waypoints/Movement/Path Direction Data")]
    public sealed class WaypointPathDirectionData : ComponentData
    {
        [OdinSerialize]
        [HideInInspector]
        private int _direction = 1;

        public int Direction
        {
            get => _direction;
            set => _direction = value >= 0 ? 1 : -1;
        }
    }
}
