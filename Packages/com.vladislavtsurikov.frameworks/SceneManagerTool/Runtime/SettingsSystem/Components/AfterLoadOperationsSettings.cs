using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.ActionFlow.Runtime;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("After Load")]
    public class AfterLoadOperationsSettings : SettingsComponent
    {
        public ActionCollection OperationStack = new();

        private object[] _setupData;

        protected override void SetupComponent(object[] setupData = null)
        {
            _setupData = setupData;
        }

        public async UniTask DoOperations(CancellationToken token = default)
        {
            OperationStack.Setup(true, _setupData);
            await OperationStack.Run(token);
        }
    }
}
