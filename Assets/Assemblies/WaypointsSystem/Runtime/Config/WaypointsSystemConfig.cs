using UnityEngine;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace ArmyClash.WaypointsSystem.Runtime.Config
{
    [LocationAsset("ArmyClash/WaypointsSystem/WaypointsSystemConfig", false)]
    public sealed class WaypointsSystemConfig : SerializedScriptableObjectSingleton<WaypointsSystemConfig>
    {
        public Color PathColor = new(0.1f, 0.8f, 1f, 1f);

        [Min(0.01f)]
        public float PointRadius = 0.2f;
    }
}
