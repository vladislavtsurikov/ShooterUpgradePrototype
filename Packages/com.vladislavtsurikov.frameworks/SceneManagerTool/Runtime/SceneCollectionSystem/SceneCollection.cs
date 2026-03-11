using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OdinSerializer;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem;
using VladislavTsurikov.SceneUtility.Runtime;
using ProgressBar = VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.ProgressBar;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SceneCollectionSystem
{
    public class SceneCollection : Node
    {
        [OdinSerialize]
        private string _name;

        [OdinSerialize]
        public SceneTypeComponentStack SceneTypeComponentStack = new();

        [OdinSerialize]
        public NodeStackOnlyDifferentTypes<SettingsComponent> SettingsStack = new();

        public bool Startup = true;

        [field: OdinSerialize]
        public int ID { get; private set; }

        public override string Name
        {
            get => _name;
            set => _name = value;
        }

        public static SceneCollection Current { get; private set; }

        public float LoadingProgress
        {
            get
            {
                var sceneComponents = new List<SceneType>();

                foreach (SceneType sceneComponent in SceneTypeComponentStack.ElementList)
                {
                    var sceneBehavior = (SceneBehavior)sceneComponent.SettingsStack.GetElement(typeof(SceneBehavior));

                    if (sceneBehavior != null)
                    {
                        if (sceneBehavior.SceneOpenBehavior == SceneOpenBehavior.DoNotOpen)
                        {
                            continue;
                        }
                    }

                    sceneComponents.Add(sceneComponent);
                }

                return !sceneComponents.Any()
                    ? 1
                    : sceneComponents.Sum(a => a.LoadingProgress()) / sceneComponents.Count;
            }
        }

        protected override void SetupComponent(object[] setupData = null)
        {
            SceneTypeComponentStack.Setup(true, new object[] { this });
            SettingsStack.Setup();
        }

        protected override void OnCreate()
        {
            ID = GetHashCode();
            _name = "Scene Collection";
        }

        public async UniTask Load(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            SceneCollection pastSceneCollection = Current;

            if (Current != null)
            {
                await FadeTransition.LoadFadeIfNecessary(Current.SettingsStack, token);
                await Current.Unload(this, token);
            }

            if (this == Current)
            {
                return;
            }

            Current = this;

            await ProgressBar.LoadProgressBarIfNecessary(SettingsStack, token);

            var beforeLoadOperationsSettings =
                (BeforeLoadOperationsSettings)SettingsStack.GetElement(typeof(BeforeLoadOperationsSettings));

            if (beforeLoadOperationsSettings != null)
            {
                await beforeLoadOperationsSettings.DoOperations(token);
            }

            await ActiveScene.LoadActiveSceneIfNecessary(SettingsStack, token);

            foreach (SceneType sceneComponent in SceneTypeComponentStack.ElementList)
            {
                token.ThrowIfCancellationRequested();
                await sceneComponent.LoadInternal(false, token);
            }

            await UniTask.WaitWhile(() => LoadingProgress != 1, cancellationToken: token);

            var afterLoadOperationsSettings =
                (AfterLoadOperationsSettings)SettingsStack.GetElement(typeof(AfterLoadOperationsSettings));

            if (afterLoadOperationsSettings != null)
            {
                await afterLoadOperationsSettings.DoOperations(token);
            }

            await ProgressBar.UnloadProgressBarIfNecessary(SettingsStack, token);

            if (pastSceneCollection != null)
            {
                await FadeTransition.UnloadFadeIfNecessary(pastSceneCollection.SettingsStack, token);
            }
        }

        public async UniTask Unload(SceneCollection nextLoadSceneCollection, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            Current = null;

            var beforeUnloadOperationsSettings =
                (BeforeUnloadOperationsSettings)SettingsStack.GetElement(typeof(BeforeUnloadOperationsSettings));

            if (beforeUnloadOperationsSettings != null)
            {
                await beforeUnloadOperationsSettings.DoOperations(token);
            }

            foreach (SceneType sceneComponent in SceneTypeComponentStack.ElementList)
            {
                token.ThrowIfCancellationRequested();
                await sceneComponent.UnloadInternal(nextLoadSceneCollection, false, token);
            }

            await ActiveScene.UnloadActiveSceneIfNecessary(SettingsStack, token);

            var afterUnloadOperationsSettings =
                (AfterUnloadOperationsSettings)SettingsStack.GetElement(typeof(AfterUnloadOperationsSettings));

            if (afterUnloadOperationsSettings != null)
            {
                await afterUnloadOperationsSettings.DoOperations(token);
            }
        }

        public bool HasScene(SceneReference sceneReference) => SceneTypeComponentStack.HasScene(sceneReference);

        public List<SceneReference> GetSceneReferences()
        {
            var sceneReferences = new List<SceneReference>();

            foreach (SceneType sceneComponent in SceneTypeComponentStack.ElementList)
            {
                foreach (SettingsComponent sceneManagerComponent in SettingsStack.ElementList)
                {
                    sceneReferences.AddRange(sceneManagerComponent.GetSceneReferences());
                }

                sceneReferences.AddRange(sceneComponent.GetSceneReferencesInternal());
            }

            return sceneReferences;
        }

        public bool IsLoaded() => Current == this;
    }
}