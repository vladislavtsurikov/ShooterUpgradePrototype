#if UI_SYSTEM_ADDRESSABLE_LOADER_SYSTEM
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.AddressableLoaderSystemIntegration
{
    public sealed class SceneUICompositionService
    {
        private readonly UIHandlerManager _handlerManager;

        public SceneUICompositionService(UIHandlerManager handlerManager) => _handlerManager = handlerManager;

        public void RemoveSceneHandlers() => _handlerManager.RemoveExceptGlobalHandlers();

        public UniTask ComposeScene(string sceneName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return UniTask.CompletedTask;
            }

            return _handlerManager.AddFilter(
                attribute => attribute is SceneFilterAttribute sceneFilter && sceneFilter.Matches(sceneName),
                cancellationToken);
        }

        public UniTask RecomposeScene(string sceneName, CancellationToken cancellationToken = default)
        {
            RemoveSceneHandlers();
            return ComposeScene(sceneName, cancellationToken);
        }
    }
}
#endif
