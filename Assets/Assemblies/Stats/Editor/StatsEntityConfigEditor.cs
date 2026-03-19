#if UNITY_EDITOR
using Stats.EntityDataActionIntegration;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.IMGUI;

namespace Stats.Editor
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
