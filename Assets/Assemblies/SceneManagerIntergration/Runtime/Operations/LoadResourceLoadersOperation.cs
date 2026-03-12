using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VladislavTsurikov.AddressableLoaderSystem.Runtime.Core;
using VladislavTsurikov.Core.Runtime;
using VladislavTsurikov.ReflectionUtility;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.SceneUtility.Runtime;
using Zenject;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;
using Single = VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem.Single;

namespace ArmyClash.SceneManager
{
    [Name("Addressables/Load Resource Loaders")]
    [ParentRequired(typeof(BeforeLoadOperationsSettings), typeof(Single))]
    public sealed class LoadResourceLoadersOperation : Action
    {
        [Inject]
        private ResourceLoaderManager _manager;

        private SceneReference _sceneReference = new();

        protected override void SetupComponent(object[] setupData = null)
        {
            Single single = ContextHierarchy.GetContext<Single>();
            if (single != null)
            {
                _sceneReference = single.SceneReference;
            }
        }

        protected override async UniTask<bool> Run(CancellationToken token)
        {
            await _manager.Load(attribute =>
            {
                if (attribute is GlobalFilterAttribute)
                {
                    return true;
                }

                if (_sceneReference != null &&
                    !string.IsNullOrEmpty(_sceneReference.SceneName) &&
                    attribute is SceneFilterAttribute sceneFilter &&
                    sceneFilter.Matches(_sceneReference.SceneName))
                {
                    return true;
                }

                return false;
            }, token);

            return true;
        }
    }
}
