using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;

namespace VladislavTsurikov.SceneManagerTool.Runtime
{
    public static class SceneManagerAPI
    {
        public static void UnloadSceneCollection(SceneCollection sceneCollection, CancellationToken token = default) =>
            sceneCollection.Unload(null, token).Forget();

        public static void LoadSceneCollection(SceneCollection sceneCollection, CancellationToken token = default) =>
            sceneCollection.Load(token).Forget();

        public static void LoadSceneType(SceneType sceneType, CancellationToken token = default) =>
            sceneType.LoadInternal(true, token).Forget();

        public static void UnloadSceneComponent(SceneType sceneType, CancellationToken token = default) =>
            sceneType.UnloadInternal(null, true, token).Forget();
    }
}