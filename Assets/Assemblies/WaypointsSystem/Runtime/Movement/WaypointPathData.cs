using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace WaypointsSystem.Runtime
{
    [Persistent]
    [Name("Waypoints/Movement/Waypoint Path Data")]
    public sealed class WaypointPathData : ComponentData
    {
        [OdinSerialize]
        [HideInInspector]
        public WaypointPath Path;
    }
}
