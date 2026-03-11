#if UNITY_EDITOR
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using VladislavTsurikov.SceneManagerTool.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    public static class SceneManagerEditorUtility
    {
        public static void SetAllScenesToDirty()
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        public static void EnterPlaymode()
        {
            SceneManagerData.MaskAsDirty();

            EnterPlaymodeAsync(CancellationToken.None).Forget();

            async UniTask EnterPlaymodeAsync(CancellationToken token)
            {
                while (EditorApplication.isCompiling)
                {
                    token.ThrowIfCancellationRequested();
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

                SceneManagerData.Instance.SceneManagerEditorData.SceneSetupManager.SaveSceneSetup();
                await StartupScene.OpenAsync();

                await UniTask.Delay(TimeSpan.FromSeconds(0.1f), true, cancellationToken: token);

                EditorApplication.EnterPlaymode();
                SceneManagerData.Instance.SceneManagerEditorData.RunAsBuildMode = true;
            }
        }
    }
}
#endif
