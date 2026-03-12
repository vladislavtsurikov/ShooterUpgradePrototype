#if UNITY_EDITOR
using ArmyClash.WaypointsSystem.Runtime;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ArmyClash.WaypointsSystem.Editor
{
    [CustomEditor(typeof(Runtime.WaypointsSystem))]
    public sealed class WaypointsSystemEditor : UnityEditor.Editor
    {
        private ReorderableList _pathsList;
        private SerializedProperty _pathsProperty;
        private Runtime.WaypointsSystem _system;

        private void OnEnable()
        {
            _system = (Runtime.WaypointsSystem)target;
            _pathsProperty = serializedObject.FindProperty("_paths");

            _pathsList = new ReorderableList(serializedObject, _pathsProperty, true, true, true, true)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Waypoint Paths"),
                elementHeight = EditorGUIUtility.singleLineHeight + 6f,
                onAddDropdownCallback = (rect, list) =>
                {
                    GenericMenu menu = new();
                    menu.AddItem(new GUIContent("Create New Path"), false, CreateNewPath);
                    menu.AddItem(new GUIContent("Add Selected Path"), false, AddSelectedPath);
                    menu.AddSeparator(string.Empty);
                    menu.AddItem(new GUIContent("Find All In Scene"), false, FindAllInScene);
                    menu.DropDown(rect);
                }
            };

            _pathsList.drawElementCallback = DrawElement;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            _pathsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = _pathsProperty.GetArrayElementAtIndex(index);

            rect.y += 2f;
            rect.height = EditorGUIUtility.singleLineHeight;

            Rect fieldRect = rect;
            fieldRect.width -= 60f;

            Rect selectRect = rect;
            selectRect.x = fieldRect.xMax + 4f;
            selectRect.width = 56f;

            EditorGUI.PropertyField(fieldRect, element, GUIContent.none);

            using (new EditorGUI.DisabledScope(element.objectReferenceValue == null))
            {
                if (GUI.Button(selectRect, "Select"))
                {
                    Selection.activeObject = element.objectReferenceValue;
                    EditorGUIUtility.PingObject(element.objectReferenceValue);
                }
            }
        }

        private void CreateNewPath()
        {
            if (_system == null)
            {
                return;
            }

            GameObject go = new("WaypointPath");
            Undo.RegisterCreatedObjectUndo(go, "Create Waypoint Path");
            go.transform.SetParent(_system.transform, worldPositionStays: false);

            WaypointPath path = go.AddComponent<WaypointPath>();

            serializedObject.Update();
            int index = _pathsProperty.arraySize;
            _pathsProperty.arraySize++;
            _pathsProperty.GetArrayElementAtIndex(index).objectReferenceValue = path;
            serializedObject.ApplyModifiedProperties();

            Selection.activeGameObject = go;
        }

        private void AddSelectedPath()
        {
            if (_system == null)
            {
                return;
            }

            WaypointPath selected = Selection.activeGameObject != null
                ? Selection.activeGameObject.GetComponent<WaypointPath>()
                : null;

            if (selected == null)
            {
                Debug.LogWarning("Select a GameObject with WaypointPath component to add it.");
                return;
            }

            serializedObject.Update();

            for (int i = 0; i < _pathsProperty.arraySize; i++)
            {
                if (_pathsProperty.GetArrayElementAtIndex(i).objectReferenceValue == selected)
                {
                    return;
                }
            }

            int index = _pathsProperty.arraySize;
            _pathsProperty.arraySize++;
            _pathsProperty.GetArrayElementAtIndex(index).objectReferenceValue = selected;
            serializedObject.ApplyModifiedProperties();
        }

        private void FindAllInScene()
        {
            if (_system == null)
            {
                return;
            }

            Undo.RecordObject(_system, "Find All Waypoint Paths");
            _system.RebuildFromScene();
            EditorUtility.SetDirty(_system);

            serializedObject.Update();
        }
    }
}
#endif
