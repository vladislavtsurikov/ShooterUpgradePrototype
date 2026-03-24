#if UI_SYSTEM_ZENJECT
using System;
using UnityEngine.SceneManagement;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public sealed class ZenjectUIDependencyResolverRegistrar : UIDependencyResolverRegistrar
    {
        public override IUIDependencyResolver GetResolver()
        {
            ProjectContext projectContext = ProjectContext.Instance;
            if (projectContext?.Container == null)
            {
                return null;
            }

            DiContainer projectContainer = projectContext.Container;
            DiContainer sceneContainer = null;

            try
            {
                SceneContextRegistry registry = projectContainer.Resolve<SceneContextRegistry>();
                if (registry != null)
                {
                    sceneContainer = registry.TryGetContainerForScene(SceneManager.GetActiveScene());
                }
            }
            catch
            {
            }

            if (sceneContainer == null || ReferenceEquals(sceneContainer, projectContainer))
            {
                return new ZenjectUIDependencyResolver(projectContainer);
            }

            return new ZenjectUIDependencyResolver(sceneContainer, projectContainer);
        }
    }
}
#endif
