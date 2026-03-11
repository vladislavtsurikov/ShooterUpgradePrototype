using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ActionFlow.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("Before Unload")]
    public class BeforeUnloadOperationsSettings : SettingsComponent
    {
        public ActionCollection OperationStack = new();

        protected override void SetupComponent(object[] setupData = null)
        {
            OperationStack.Setup(true, setupData);
        }

        public async UniTask DoOperations(CancellationToken token = default)
        {
            await OperationStack.Run(token);
        }
    }
}
