#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.ActionFlow.Editor.Stats
{
    [CustomEditor(typeof(StatModifier))]
    public sealed class StatModifierEditor : UnityEditor.Editor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new();
        private StatModifier _modifier;

        public override void OnInspectorGUI()
        {
            _modifier ??= target as StatModifier;
            if (_modifier == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            float fieldsHeight = _fieldsDrawer.GetFieldsHeight(_modifier);
            Rect fieldsRect = EditorGUILayout.GetControlRect(false, fieldsHeight);
            _fieldsDrawer.DrawFields(_modifier, fieldsRect);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_modifier);
            }
        }
    }
}
#endif
