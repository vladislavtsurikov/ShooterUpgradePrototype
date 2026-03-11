using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem
{
    [Name("Single Scene")]
    public class Single : SceneType
    {
        public SceneReference SceneReference = new();

        public override string Name
        {
            get
            {
                if (!SceneReference.IsValid())
                {
                    return "Set Scene [Single Scene]";
                }

                return SceneReference.SceneName + " [Single Scene]";
            }
        }

        protected override async UniTask Load(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await SceneReference.LoadScene();
        }

        protected override async UniTask Unload(SceneCollection nextLoadSceneCollection, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await UnloadSceneReference(nextLoadSceneCollection, SceneReference, token);
        }

        public override bool HasScene(SceneReference sceneReference) =>
            SceneReference.SceneName == sceneReference.SceneName;

        protected override List<SceneReference> GetSceneReferences() => new() { SceneReference };

        public override float LoadingProgress() => SceneReference.LoadingProgress;
    }
}