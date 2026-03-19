#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using WaypointsSystem.Runtime;

namespace WaypointsSystem.Editor
{
    public static class WaypointsSetupEditorExtension
    {
        [MenuItem("GameObject/ArmyClash/Waypoints/Create Waypoint Path (Setup)", false, 10)]
        public static void CreateWaypointPath(MenuCommand menuCommand)
        {
            // Ensure global config asset exists.
            _ = WaypointsSystemConfig.Instance;

            Transform parent = (menuCommand.context as GameObject)?.transform;
            global::WaypointsSystem.Runtime.WaypointsSystem waypointsSystem = Object.FindFirstObjectByType<global::WaypointsSystem.Runtime.WaypointsSystem>();

            GameObject loopPath = CreatePath(
                waypointsSystem,
                parent,
                "WaypointPath Small Loop",
                new Vector3(-6f, 0f, 0f),
                true,
                new[]
                {
                    new Vector3(-1f, 0f, -1f),
                    new Vector3(-1f, 0f, 1f),
                    new Vector3(1f, 0f, 1f),
                    new Vector3(1f, 0f, -1f)
                });

            CreatePath(
                waypointsSystem,
                parent,
                "WaypointPath Small Sphere",
                Vector3.zero,
                false,
                BuildCirclePoints(1.5f, 10));

            CreatePath(
                waypointsSystem,
                parent,
                "WaypointPath Large Sphere",
                new Vector3(6f, 0f, 0f),
                false,
                BuildCirclePoints(3f, 10));

            EditorSceneManager.MarkSceneDirty(loopPath.scene);
            Selection.activeGameObject = loopPath;
        }

        [MenuItem("GameObject/ArmyClash/Waypoints/Create Waypoint Path (Setup)", true)]
        private static bool ValidateCreateWaypointPath() => !Application.isPlaying;

        [MenuItem("Tools/ArmyClash/Waypoints/Setup Default Config")]
        public static void SetupDefaultConfig()
        {
            WaypointsSystemConfig config = WaypointsSystemConfig.Instance;
            Selection.activeObject = config;
            EditorGUIUtility.PingObject(config);
            Debug.Log($"Waypoints config is ready: {AssetDatabase.GetAssetPath(config)}");
        }

        private static GameObject CreatePath(
            global::WaypointsSystem.Runtime.WaypointsSystem waypointsSystem,
            Transform parent,
            string objectName,
            Vector3 localPosition,
            bool loop,
            IReadOnlyList<Vector3> points)
        {
            GameObject pathObject = new(objectName);
            Undo.RegisterCreatedObjectUndo(pathObject, "Create Waypoint Path");
            if (parent != null)
            {
                pathObject.transform.SetParent(parent, false);
                pathObject.transform.localPosition = localPosition;
            }
            else
            {
                pathObject.transform.position = localPosition;
            }

            WaypointPath waypointPath = pathObject.AddComponent<WaypointPath>();
            Undo.RecordObject(waypointPath, "Setup Waypoint Path");
            if (waypointsSystem != null)
            {
                Undo.RecordObject(waypointsSystem, "Add Waypoint Path");
                waypointsSystem.AddPath(waypointPath);
                EditorUtility.SetDirty(waypointsSystem);
            }

            //waypointPath.GetData<LoopData>().Loop.Value = loop;

            for (int i = 0; i < points.Count; i++)
            {
                waypointPath.AddPoint(points[i]);
            }

            EditorUtility.SetDirty(waypointPath);
            return pathObject;
        }

        private static Vector3[] BuildCirclePoints(float radius, int pointsCount)
        {
            Vector3[] points = new Vector3[Mathf.Max(3, pointsCount)];
            float step = 360f / points.Length;

            for (int i = 0; i < points.Length; i++)
            {
                float angle = step * i * Mathf.Deg2Rad;
                points[i] = new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);
            }

            return points;
        }
    }
}
#endif
