#if UNITY_EDITOR
using System.Collections.Generic;
using AutoStrike.Input.Services;
using UnityEditor;

namespace AutoStrike.Input.Editor
{
    [CustomEditor(typeof(InputModePreferences))]
    public sealed class InputModePreferencesEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI() => DrawSettingsGUI();

        [SettingsProvider]
        public static SettingsProvider PreferencesGUI()
        {
            var provider = new SettingsProvider("Preferences/AutoStrike/Input", SettingsScope.User)
            {
                label = "AutoStrike Input",
                guiHandler = _ => DrawSettingsGUI(),
                keywords = new HashSet<string>(new[] { "Input", "Control", "Mode", "Desktop", "Mobile", "Gamepad" })
            };

            return provider;
        }

        private static void DrawSettingsGUI()
        {
            InputModePreferences settings = InputModePreferences.Instance;
            if (settings == null)
            {
                EditorGUILayout.HelpBox("InputModePreferences asset is missing.", MessageType.Warning);
                return;
            }

            EditorGUI.BeginChangeCheck();

            string[] preferences = InputModeIds.Preferences;
            string currentPreference = InputModePreferences.CurrentPreference;
            int currentIndex = System.Array.IndexOf(preferences, currentPreference);
            if (currentIndex < 0)
            {
                currentIndex = 0;
            }

            int selectedIndex = EditorGUILayout.Popup(
                "Play Mode Input",
                currentIndex,
                preferences);

            if (EditorGUI.EndChangeCheck())
            {
                InputModePreferences.SetPlayModePreference(preferences[selectedIndex]);
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
#endif
