#if UNITY_EDITOR
using ArmyClash.WaypointsSystem.Runtime;
using ArmyClash.WaypointsSystem.Runtime.Config;
using ArmyClash.WaypointsSystem.Runtime.Spawning;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.EntityDataAction.Editor.Core;

namespace ArmyClash.WaypointsSystem.Editor
{
    [CustomEditor(typeof(WaypointPath))]
    public sealed class WaypointPathEditor : UnityEditor.Editor
    {
        private const float MinDrawRadius = 0.05f;

        private readonly EntityMonoBehaviourInspectorDrawer _entityInspectorDrawer = new();
        private WaypointPath _path;
        private int _selectedIndex = -1;

        private void OnEnable()
        {
            _path = (WaypointPath)target;
            _entityInspectorDrawer.Setup(_path);
            ClampSelectedIndex();
        }

        public override void OnInspectorGUI()
        {
            _path ??= target as WaypointPath;
            if (_path == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();

            _entityInspectorDrawer.DrawInspector();
            EditorGUILayout.Space(8f);

            DrawToolbar();
            DrawPointsList();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_path);
            }
        }

        private void OnSceneGUI()
        {
            if (_path == null || _path.Count == 0)
            {
                return;
            }

            ClampSelectedIndex();
            DrawPath();
            DrawPointButtons();
            DrawSelectedPointHandle();
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Add Waypoint"))
                {
                    AddWaypoint();
                }

                using (new EditorGUI.DisabledScope(!HasValidSelection()))
                {
                    if (GUILayout.Button("Remove Selected"))
                    {
                        RemoveSelectedWaypoint();
                    }
                }
            }
        }

        private void DrawPointsList()
        {
            EditorGUILayout.LabelField("Points", EditorStyles.boldLabel);

            if (_path.Count == 0)
            {
                EditorGUILayout.HelpBox("No points yet. Click Add Waypoint to create one.",
                    MessageType.Info);
                return;
            }

            for (int i = 0; i < _path.Count; i++)
            {
                Vector3 currentValue = _path.GetLocalPoint(i);

                using (new EditorGUILayout.HorizontalScope())
                {
                    bool isSelected = _selectedIndex == i;
                    if (GUILayout.Toggle(isSelected, $"#{i}", "Button", GUILayout.Width(40f)))
                    {
                        _selectedIndex = i;
                    }

                    EditorGUI.BeginChangeCheck();
                    Vector3 newValue = EditorGUILayout.Vector3Field(GUIContent.none, currentValue);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(_path, "Move Waypoint");
                        _path.SetLocalPoint(i, newValue);
                        EditorUtility.SetDirty(_path);
                    }
                }
            }
        }

        private void DrawPath()
        {
            Color previousColor = Handles.color;
            Handles.color = WaypointsSystemConfig.Instance.PathColor;
            LoopData loopData = _path.GetData<LoopData>();
            bool loop = loopData == null || loopData.Loop.Value;

            for (int i = 0; i < _path.Count - 1; i++)
            {
                Vector3 from = _path.GetWorldPoint(i);
                Vector3 to = _path.GetWorldPoint(i + 1);
                Handles.DrawAAPolyLine(3f, from, to);
            }

            if (loop && _path.Count > 2)
            {
                Handles.DrawAAPolyLine(3f, _path.GetWorldPoint(_path.Count - 1), _path.GetWorldPoint(0));
            }

            Handles.color = previousColor;
        }

        private void DrawPointButtons()
        {
            for (int i = 0; i < _path.Count; i++)
            {
                Vector3 worldPoint = _path.GetWorldPoint(i);
                float handleSize = HandleUtility.GetHandleSize(worldPoint);
                float radius = handleSize * Mathf.Max(MinDrawRadius, WaypointsSystemConfig.Instance.PointRadius);

                Color previousColor = Handles.color;
                Handles.color = _selectedIndex == i ? Color.yellow : WaypointsSystemConfig.Instance.PathColor;
                if (Handles.Button(worldPoint, Quaternion.identity, radius, radius, Handles.SphereHandleCap))
                {
                    _selectedIndex = i;
                    Repaint();
                }

                Handles.color = previousColor;
                Handles.Label(worldPoint + Vector3.up * radius, i.ToString());
            }
        }

        private void DrawSelectedPointHandle()
        {
            if (!HasValidSelection())
            {
                return;
            }

            Vector3 worldPoint = _path.GetWorldPoint(_selectedIndex);

            EditorGUI.BeginChangeCheck();
            Vector3 movedPoint = Handles.PositionHandle(worldPoint, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_path, "Move Waypoint");
                _path.SetLocalPoint(_selectedIndex, _path.transform.InverseTransformPoint(movedPoint));
                EditorUtility.SetDirty(_path);
            }
        }

        private void AddWaypoint()
        {
            Undo.RecordObject(_path, "Add Waypoint");
            _path.AddPoint(GetNextLocalPoint());
            _selectedIndex = _path.Count - 1;
            EditorUtility.SetDirty(_path);
        }

        private void RemoveSelectedWaypoint()
        {
            if (!HasValidSelection())
            {
                return;
            }

            Undo.RecordObject(_path, "Remove Waypoint");
            _path.RemovePoint(_selectedIndex);
            ClampSelectedIndex();
            EditorUtility.SetDirty(_path);
        }

        private Vector3 GetNextLocalPoint()
        {
            if (_path.Count == 0)
            {
                return _path.transform.InverseTransformPoint(_path.transform.position + _path.transform.forward);
            }

            return _path.GetLocalPoint(_path.Count - 1) + Vector3.forward;
        }

        private bool HasValidSelection() => _selectedIndex >= 0 && _selectedIndex < _path.Count;

        private void ClampSelectedIndex()
        {
            if (_path == null || _path.Count == 0)
            {
                _selectedIndex = -1;
                return;
            }

            _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _path.Count - 1);
        }
    }
}
#endif
