#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;

namespace VladislavTsurikov.EntityDataAction.Shared.Editor.Stats
{
    [CustomEditor(typeof(StatsEntityConfig))]
    public sealed class StatsEntityConfigEditor : UnityEditor.Editor
    {
        private readonly IMGUIInspectorFieldsDrawer _fieldsDrawer = new();

        private StatsEntityConfig _config;

        private void OnEnable()
        {
            _config = target as StatsEntityConfig;
        }

        public override void OnInspectorGUI()
        {
            if (_config == null)
            {
                return;
            }

            EditorGUI.BeginChangeCheck();

            float fieldsHeight = _fieldsDrawer.GetFieldsHeight(_config);
            Rect fieldsRect = EditorGUILayout.GetControlRect(false, fieldsHeight);
            _fieldsDrawer.DrawFields(_config, fieldsRect);

            if (EditorGUI.EndChangeCheck())
            {
                _config.Stats.RebuildFromCollection();
                EditorUtility.SetDirty(_config);
            }
        }
    }
}
#endif
