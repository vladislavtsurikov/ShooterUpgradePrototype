using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.Callbacks.SceneOperation;
using VladislavTsurikov.SceneUtility.Runtime;
using GameObjectUtility = VladislavTsurikov.UnityUtility.Runtime.GameObjectUtility;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
#endif

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("Progress Bar")]
    [SceneCollectionComponent]
    public class ProgressBar : SettingsComponent
    {
        public bool DisableFade = false;
        public SceneReference SceneReference = new();

#if UNITY_EDITOR
        protected override void OnCreate()
        {
            SceneAsset sceneAsset = Resources.Load<SceneAsset>("Progress Bar");

            if (sceneAsset != null)
            {
                SceneReference = new SceneReference(sceneAsset);
            }
        }
#endif

        internal static async UniTask LoadProgressBarIfNecessary(
            NodeStackOnlyDifferentTypes<SettingsComponent> settingsList,
            CancellationToken token = default)
        {
            var progressBar = (ProgressBar)settingsList.GetElement(typeof(ProgressBar));

            if (progressBar != null)
            {
                await progressBar.LoadFade(token);
            }
        }

        internal static async UniTask UnloadProgressBarIfNecessary(
            NodeStackOnlyDifferentTypes<SettingsComponent> settingsList,
            CancellationToken token = default)
        {
            var progressBar = (ProgressBar)settingsList.GetElement(typeof(ProgressBar));

            if (progressBar != null)
            {
                await progressBar.UnloadFade(token);
            }
        }

        private async UniTask LoadFade(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await SceneReference.LoadScene();

            var sceneOperation =
                (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];

            await sceneOperation.OnLoad(token);
        }

        private async UniTask UnloadFade(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var sceneOperation =
                (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];

            await sceneOperation.OnUnload(token);

            token.ThrowIfCancellationRequested();
            await SceneReference.UnloadScene();
        }

        public override List<SceneReference> GetSceneReferences() => new() { SceneReference };
    }
}