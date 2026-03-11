#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Editor.Core
{
    [CustomEditor(typeof(EntityDataActionGlobalSettings))]
    public sealed class EntityDataActionGlobalSettingsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() => DrawSettingsGUI();

        [SettingsProvider]
        public static SettingsProvider PreferencesGUI()
        {
            var provider = new SettingsProvider("Preferences/Entity Data Action", SettingsScope.User)
            {
                label = "Entity Data Action",
                guiHandler = _ => DrawSettingsGUI(),
                keywords = new HashSet<string>(new[] { "Entity", "Data", "Action", "Edit Mode" })
            };

            return provider;
        }

        private static void DrawSettingsGUI()
        {
            EntityDataActionGlobalSettings settings = EntityDataActionGlobalSettings.Instance;
            if (settings == null)
            {
                EditorGUILayout.HelpBox("EntityDataActionGlobalSettings asset is missing.", MessageType.Warning);
                return;
            }

            EditorGUI.BeginChangeCheck();

            bool activeInEditMode = EditorGUILayout.Toggle("Active In Edit Mode", EntityDataActionGlobalSettings.Active);

            if (EditorGUI.EndChangeCheck())
            {
                EntityDataActionGlobalSettings.Active = activeInEditMode;
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
#endif
