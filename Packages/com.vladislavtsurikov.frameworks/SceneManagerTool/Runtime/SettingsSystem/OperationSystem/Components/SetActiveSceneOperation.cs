using UnityEngine.SceneManagement;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneUtility.Runtime;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;
using Single = VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem.Single;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.OperationSystem
{
    [Name("Scene/Set Active Scene")]
    [ParentRequired(typeof(AfterLoadOperationsSettings), typeof(Single))]
    public sealed class SetActiveSceneOperation : Action
    {
        private SceneReference _sceneReference = new();

        protected override void SetupComponent(object[] setupData = null)
        {
            Single single = ContextHierarchy.GetContext<Single>();
            _sceneReference = single.SceneReference;

            Scene scene = _sceneReference.Scene;
            SceneManager.SetActiveScene(scene);
        }
    }
}
