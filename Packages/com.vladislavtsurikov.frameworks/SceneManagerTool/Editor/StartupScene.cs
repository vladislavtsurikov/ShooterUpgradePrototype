#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using UnityEditor;
using UnityEditor.SceneManagement;
using VladislavTsurikov.SceneManagerTool.Runtime;
using VladislavTsurikov.SceneUtility.Editor;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Editor
{
    [Serializable]
    public sealed class StartupScene
    {
        [OdinSerialize]
        private SceneReference _sceneReference = new();

        public void Setup()
        {
            if (_sceneReference.IsValid())
            {
                return;
            }

            EditorApplication.delayCall += () =>
            {
                _sceneReference = new SceneReference(SceneCreationUtility.CreateScene("Scene Manager",
                    SceneManagerPath.PathToResourcesSceneManager));
            };
        }

        public static void Open() => OpenAsync().Forget();

        public static async UniTask OpenAsync()
        {
            string path = await GetStartupScenePathAsync();
            if (!string.IsNullOrEmpty(path))
            {
                EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            }
        }

        public static async UniTask<string> GetStartupScenePathAsync()
        {
            if (!SceneManagerData.Instance.SceneManagerEditorData.StartupScene._sceneReference.IsValid())
            {
                SceneManagerData.Instance.SceneManagerEditorData.StartupScene.Setup();
            }

            while (!SceneManagerData.Instance.SceneManagerEditorData.StartupScene._sceneReference.IsValid())
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            return SceneManagerData.Instance.SceneManagerEditorData.StartupScene._sceneReference.ScenePath;
        }

        public static bool TryGetStartupScenePath(out string path)
        {
            if (SceneManagerData.Instance.SceneManagerEditorData.StartupScene._sceneReference.IsValid())
            {
                path = SceneManagerData.Instance.SceneManagerEditorData.StartupScene._sceneReference.ScenePath;
                return true;
            }

            path = string.Empty;
            return false;
        }
    }
}
#endif
