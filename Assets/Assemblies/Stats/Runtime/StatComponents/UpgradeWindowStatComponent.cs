using Nody.Runtime.Core;
using OdinSerializer;
using VladislavTsurikov.EntityDataAction.Shared.Runtime.Stats;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
{
    [Name("Stats/Upgrade Window")]
    [Group("Stats")]
    public sealed class UpgradeWindowStatComponent : StatComponentData
    {
        [OdinSerialize]
        private int _order;

        public int Order => _order;

        public override RuntimeStatData CreateRuntimeComponent() => null;
    }
}
