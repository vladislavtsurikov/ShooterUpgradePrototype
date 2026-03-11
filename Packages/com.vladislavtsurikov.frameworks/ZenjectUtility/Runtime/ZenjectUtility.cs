using UnityEngine.SceneManagement;
using Zenject;

namespace VladislavTsurikov.ZenjectUtility.Runtime
{
    public static class ZenjectUtility
    {
        public static bool IsSceneContextReady()
        {
            ProjectContext projectContext = ProjectContext.Instance;
            if (projectContext == null || projectContext.Container == null)
            {
                return false;
            }

            SceneContextRegistry registry = projectContext.Container.Resolve<SceneContextRegistry>();
            if (registry == null)
            {
                return false;
            }

            return registry.TryGetContainerForScene(SceneManager.GetActiveScene()) != null;
        }
    }
}
