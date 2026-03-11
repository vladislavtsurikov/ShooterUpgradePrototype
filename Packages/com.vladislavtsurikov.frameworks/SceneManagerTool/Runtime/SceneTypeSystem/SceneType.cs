using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem
{
    public enum SceneCloseBehavior
    {
        Close,
        KeepOpenAlways
    }

    public enum SceneOpenBehavior
    {
        Open,
        DoNotOpen
    }

    public abstract class SceneType : Node
    {
        private SceneCollection _sceneCollection;

        public NodeStackOnlyDifferentTypes<SettingsComponent> SettingsStack = new();

        protected override void SetupComponent(object[] setupData = null)
        {
            _sceneCollection = (SceneCollection)setupData[0];

            SettingsStack.Setup();
        }

        internal async UniTask LoadInternal(bool force = false, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var sceneBehavior = (SceneBehavior)SettingsStack.GetElement(typeof(SceneBehavior));

            if (!force && sceneBehavior is { SceneOpenBehavior: SceneOpenBehavior.DoNotOpen })
            {
                return;
            }

            var beforeLoadOperationsSettings =
                (BeforeLoadOperationsSettings)SettingsStack.GetElement(typeof(BeforeLoadOperationsSettings));

            if (beforeLoadOperationsSettings != null)
            {
                await beforeLoadOperationsSettings.DoOperations(token);
            }

            await Load(token);

            var afterLoadOperationsSettings =
                (AfterLoadOperationsSettings)SettingsStack.GetElement(typeof(AfterLoadOperationsSettings));

            if (afterLoadOperationsSettings != null)
            {
                await afterLoadOperationsSettings.DoOperations(token);
            }
        }

        internal async UniTask UnloadInternal(
            SceneCollection nextLoadSceneCollection = null,
            bool force = false,
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var sceneBehavior = (SceneBehavior)SettingsStack.GetElement(typeof(SceneBehavior));

            if (!force && sceneBehavior is { SceneCloseBehavior: SceneCloseBehavior.KeepOpenAlways })
            {
                return;
            }

            var beforeUnloadOperationsSettings =
                (BeforeUnloadOperationsSettings)SettingsStack.GetElement(typeof(BeforeUnloadOperationsSettings));

            if (beforeUnloadOperationsSettings != null)
            {
                await beforeUnloadOperationsSettings.DoOperations(token);
            }

            await Unload(nextLoadSceneCollection, token);
        }

        internal async UniTask UnloadSceneReference(
            SceneCollection nextLoadSceneCollection,
            SceneReference scene,
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (nextLoadSceneCollection == null)
            {
                await scene.UnloadScene();
            }
            else if (!nextLoadSceneCollection.HasScene(scene))
            {
                await scene.UnloadScene();
            }
        }

        internal List<SceneReference> GetSceneReferencesInternal()
        {
            var sceneReferences = new List<SceneReference>();

            foreach (SettingsComponent component in SettingsStack.ElementList)
            {
                sceneReferences.AddRange(component.GetSceneReferences());
            }

            sceneReferences.AddRange(GetSceneReferences());

            return sceneReferences;
        }

        protected abstract UniTask Load(CancellationToken token);
        protected abstract UniTask Unload(SceneCollection nextLoadSceneCollection, CancellationToken token);
        public abstract bool HasScene(SceneReference sceneReference);
        protected abstract List<SceneReference> GetSceneReferences();
        public abstract float LoadingProgress();
    }
}