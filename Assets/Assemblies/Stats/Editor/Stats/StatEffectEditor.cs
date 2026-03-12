#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ActionFlow.Runtime.Stats;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace VladislavTsurikov.ActionFlow.Editor.Stats
{
    [CustomEditor(typeof(StatEffect), true)]
    public sealed class StatEffectEditor : UnityEditor.Editor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new();
        private StatEffect _statEffect;

        private void OnEnable()
        {
            _statEffect = target as StatEffect;
        }

        public override void OnInspectorGUI()
        {
            if (_statEffect == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();
            float fieldsHeight = _fieldsDrawer.GetFieldsHeight(_statEffect);
            Rect fieldsRect = EditorGUILayout.GetControlRect(false, fieldsHeight);
            _fieldsDrawer.DrawFields(_statEffect, fieldsRect);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_statEffect);
            }
        }
    }
}
#endif
