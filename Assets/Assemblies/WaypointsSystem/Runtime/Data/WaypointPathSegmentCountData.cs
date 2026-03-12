using Nody.Runtime.Core;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace ArmyClash.WaypointsSystem.Runtime.Spawning
{
    [Persistent]
    [Name("Waypoints/Spawning/Segment Count Data")]
    [Group("Waypoints")]
    public sealed class WaypointPathSegmentCountData : ComponentData
    {
        private int _segmentCount;

        public int SegmentCount => _segmentCount;

        public void Rebuild(WaypointPath path)
        {
            bool loop = path.GetData<LoopData>().Loop.Value;
            int pointCount = path.Count;

            if (pointCount <= 1)
            {
                _segmentCount = 0;
                return;
            }

            _segmentCount = loop ? pointCount : pointCount - 1;
        }
    }
}
