#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using WaypointsSystem.Runtime;

namespace WaypointsSystem.Editor
{
    public static class WaypointPathVisualisation
    {
        [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
        private static void DrawGizmo(WaypointPath path, GizmoType gizmoType)
        {
            if (path == null || path.Count == 0)
            {
                return;
            }

            DrawPath(path);
            DrawPoints(path);
        }

        private static void DrawPath(WaypointPath path)
        {
            Color previousColor = Handles.color;
            Handles.color = WaypointsSystemConfig.Instance.PathColor;

            LoopData loopData = path.GetData<LoopData>();
            bool loop = loopData == null || loopData.Loop.Value;

            for (int i = 0; i < path.Count - 1; i++)
            {
                Handles.DrawAAPolyLine(3f, path.GetWorldPoint(i), path.GetWorldPoint(i + 1));
            }

            if (loop && path.Count > 2)
            {
                Handles.DrawAAPolyLine(3f, path.GetWorldPoint(path.Count - 1), path.GetWorldPoint(0));
            }

            Handles.color = previousColor;
        }

        private static void DrawPoints(WaypointPath path)
        {
            Color previousColor = Handles.color;
            Handles.color = WaypointsSystemConfig.Instance.PathColor;

            for (int i = 0; i < path.Count; i++)
            {
                Vector3 worldPoint = path.GetWorldPoint(i);
                float handleSize = HandleUtility.GetHandleSize(worldPoint);
                float radius = handleSize * Mathf.Max(0.05f, WaypointsSystemConfig.Instance.PointRadius);

                Handles.SphereHandleCap(0, worldPoint, Quaternion.identity, radius, EventType.Repaint);
                Handles.Label(worldPoint + Vector3.up * radius, i.ToString());
            }

            Handles.color = previousColor;
        }
    }
}
#endif
