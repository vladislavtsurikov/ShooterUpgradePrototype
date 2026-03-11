using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem
{
    [Name("Group")]
    public class Group : SceneType
    {
        public List<SceneReference> SceneReferences = new();

        protected override async UniTask Load(CancellationToken token)
        {
            foreach (SceneReference sceneReference in SceneReferences)
            {
                token.ThrowIfCancellationRequested();
                await sceneReference.LoadScene();
            }
        }

        protected override async UniTask Unload(SceneCollection nextLoadSceneCollection, CancellationToken token)
        {
            foreach (SceneReference sceneReference in SceneReferences)
            {
                token.ThrowIfCancellationRequested();
                await UnloadSceneReference(nextLoadSceneCollection, sceneReference, token);
            }
        }

        public override bool HasScene(SceneReference sceneReference) =>
            SceneReferences.FindAll(reference => reference.SceneName == sceneReference.SceneName).Count != 0;

        protected override List<SceneReference> GetSceneReferences() => new(SceneReferences);

        public override float LoadingProgress() => !SceneReferences.Any()
            ? 1
            : SceneReferences.Sum(a => a.LoadingProgress) / SceneReferences.Count;
    }
}