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
    [Name("Fade Transition")]
    [SceneCollectionComponent]
    public class FadeTransition : SettingsComponent
    {
        public SceneReference SceneReference = new();

        internal static async UniTask LoadFadeIfNecessary(
            NodeStackOnlyDifferentTypes<SettingsComponent> settingsList,
            CancellationToken token = default)
        {
            var fadeTransition = (FadeTransition)settingsList.GetElement(typeof(FadeTransition));

            if (fadeTransition != null)
            {
                await fadeTransition.LoadFadeIfNecessary(token);
            }
        }

        internal static async UniTask UnloadFadeIfNecessary(
            NodeStackOnlyDifferentTypes<SettingsComponent> settingsList,
            CancellationToken token = default)
        {
            var fadeTransition = (FadeTransition)settingsList.GetElement(typeof(FadeTransition));

            if (fadeTransition != null)
            {
                await fadeTransition.UnloadFadeIfNecessary(token);
            }
        }

#if UNITY_EDITOR
        protected override void OnCreate()
        {
            SceneAsset sceneAsset = Resources.Load<SceneAsset>("Fade");

            if (sceneAsset != null)
            {
                SceneReference = new SceneReference(sceneAsset);
            }
        }
#endif

        private async UniTask LoadFadeIfNecessary(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await SceneReference.LoadScene();

            var sceneOperation =
                (SceneOperation)GameObjectUtility.FindObjectsOfType(typeof(SceneOperation), SceneReference.Scene)[0];

            await sceneOperation.OnLoad(token);
        }

        private async UniTask UnloadFadeIfNecessary(CancellationToken token)
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