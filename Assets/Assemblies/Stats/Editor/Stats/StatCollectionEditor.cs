#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.ActionFlow.Editor.Stats
{
    [CustomEditor(typeof(StatCollection))]
    public sealed class StatCollectionEditor : UnityEditor.Editor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new();
        private StatCollection _collection;

        public override void OnInspectorGUI()
        {
            _collection ??= target as StatCollection;
            if (_collection == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            float fieldsHeight = _fieldsDrawer.GetFieldsHeight(_collection);
            Rect fieldsRect = EditorGUILayout.GetControlRect(false, fieldsHeight);
            _fieldsDrawer.DrawFields(_collection, fieldsRect);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_collection);
            }
        }
    }
}
#endif
