#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ShooterUpgradePrototype.Editor
{
    public static class PlayerPrefsTools
    {
        [MenuItem("Tools/Clear PlayerPrefs")]
        private static void ClearPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs cleared");
        }
    }
}
#endif
