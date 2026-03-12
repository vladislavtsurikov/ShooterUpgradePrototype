using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.ActionFlow.Runtime.Stats
{
    public sealed class StatsComponentStack : NodeStackOnlyDifferentTypes<ComponentData>
    {
        protected override void OnSetup()
        {
            AllowedGroup.Set(new[] { "Stats", "CommonUI" });
        }
    }
}
