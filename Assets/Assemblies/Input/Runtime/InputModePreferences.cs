using System;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace AutoStrike.Input.Services
{
    [LocationAsset("AutoStrike/Input/InputModePreferences", false)]
    public sealed class InputModePreferences : SerializedScriptableObjectSingleton<InputModePreferences>
    {
        public string PlayModePreference = InputModeIds.Auto;

        public static string CurrentPreference => UnityEngine.Application.isEditor
            ? InputModeIds.NormalizePreference(Instance.PlayModePreference)
            : InputModeIds.Auto;

        public static event Action<string> PreferenceChanged;

        public static void SetPlayModePreference(string preference)
        {
            if (!UnityEngine.Application.isEditor)
            {
                return;
            }

            string normalizedPreference = InputModeIds.NormalizePreference(preference);

            if (Instance.PlayModePreference == normalizedPreference)
            {
                return;
            }

            Instance.PlayModePreference = normalizedPreference;
            PreferenceChanged?.Invoke(normalizedPreference);
        }
    }
}
