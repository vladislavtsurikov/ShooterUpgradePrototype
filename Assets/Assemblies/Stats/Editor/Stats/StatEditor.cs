#if UNITY_EDITOR
using Stats.Runtime;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.Nody.Runtime.Core;

namespace Stats.Editor.Stats
{
    [CustomEditor(typeof(Stat))]
    public sealed class StatEditor : UnityEditor.Editor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new();
        private ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor> _stackEditor;
        private Stat _stat;

        private void OnEnable()
        {
            _stat ??= target as Stat;

            if (_stackEditor != null)
            {
                return;
            }

            _stackEditor =
                new ReorderableListStackEditor<ComponentData, ReorderableListComponentEditor>(_stat.ComponentStack)
                {
                    DisplayHeaderText = true
                };
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            float fieldsHeight = _fieldsDrawer.GetFieldsHeight(_stat);
            Rect fieldsRect = EditorGUILayout.GetControlRect(false, fieldsHeight);
            _fieldsDrawer.DrawFields(_stat, fieldsRect);

            _stackEditor.OnGUI();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(_stat);
            }
        }
    }
}
#endif
