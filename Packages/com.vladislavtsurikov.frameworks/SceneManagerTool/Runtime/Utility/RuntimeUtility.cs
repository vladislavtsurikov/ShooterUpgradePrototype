using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime.Utility
{
    internal static class RuntimeUtility
    {
        internal static void Start(CancellationToken token = default)
        {
            Load(token).Forget();

            static async UniTask Load(CancellationToken token)
            {
                foreach (SceneCollection sceneCollection in SceneManagerData.Instance.Profile.BuildSceneCollectionStack
                             .ActiveBuildSceneCollection.GetStartupSceneCollections())
                {
                    token.ThrowIfCancellationRequested();
                    await sceneCollection.Load(token);
                }
            }
        }
    }
}