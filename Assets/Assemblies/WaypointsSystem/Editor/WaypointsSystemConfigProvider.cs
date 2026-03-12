#if UNITY_EDITOR
using System.Collections.Generic;
using ArmyClash.WaypointsSystem.Runtime.Config;
using UnityEditor;
using UnityEngine;

namespace ArmyClash.WaypointsSystem.Editor
{
    public static class WaypointsSystemConfigProvider
    {
        [SettingsProvider]
        public static SettingsProvider SettingsGUI()
        {
            SettingsProvider provider = new("Preferences/WaypointsSystem", SettingsScope.User)
            {
                label = "WaypointsSystem",
                guiHandler = _ =>
                {
                    WaypointsSystemConfig settings = WaypointsSystemConfig.Instance;
                    if (settings == null)
                    {
                        EditorGUILayout.HelpBox("WaypointsSystemConfig asset is missing.", MessageType.Warning);
                        return;
                    }

                    settings.PathColor = EditorGUILayout.ColorField("Path Color", settings.PathColor);
                    settings.PointRadius = EditorGUILayout.FloatField("Point Radius", settings.PointRadius);

                    if (GUI.changed)
                    {
                        EditorUtility.SetDirty(settings);
                    }
                },
                keywords = new HashSet<string>(new[] { "Waypoints", "Waypoint", "Gizmo", "Path" })
            };

            return provider;
        }
    }
}
#endif

