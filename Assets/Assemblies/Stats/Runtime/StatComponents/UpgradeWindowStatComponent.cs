using Nody.Runtime.Core;
using OdinSerializer;
using Stats.EntityDataActionIntegration;
using VladislavTsurikov.ReflectionUtility;

namespace Stats.Runtime.StatComponents
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
