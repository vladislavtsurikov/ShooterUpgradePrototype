using Nody.Runtime.Core;
using OdinSerializer;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace Stats.Runtime.StatComponents
{
    [Name("Stats/Upgrade Window")]
    [Group("Stats")]
    public sealed class UpgradeWindowStatComponent : ComponentData
    {
        [OdinSerialize]
        private int _order;

        public int Order => _order;
    }
}
