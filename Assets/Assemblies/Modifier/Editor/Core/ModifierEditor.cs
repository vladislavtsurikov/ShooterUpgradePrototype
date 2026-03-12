#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.ActionFlow.Editor.Modifier
{
    [CustomEditor(typeof(Runtime.Modifier.Modifier), true)]
    public sealed class ModifierEditor : UnityEditor.Editor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new();
        private Runtime.Modifier.Modifier _modifier;
        private ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor> _stackEditor;

        private void OnEnable()
        {
            _modifier = target as Runtime.Modifier.Modifier;

            _stackEditor =
                new ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor>(_modifier.ComponentStack)
                {
                    DisplayHeaderText = true
                };
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            float fieldsHeight = _fieldsDrawer.GetFieldsHeight(_modifier);
            Rect fieldsRect = EditorGUILayout.GetControlRect(false, fieldsHeight);
            _fieldsDrawer.DrawFields(_modifier, fieldsRect);

            _stackEditor.OnGUI();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}
#endif
