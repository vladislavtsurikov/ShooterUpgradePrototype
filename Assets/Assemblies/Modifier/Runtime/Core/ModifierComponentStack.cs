using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.ActionFlow.Runtime.Modifier
{
    public sealed class ModifierComponentStack : NodeStackOnlyDifferentTypes<ComponentData>
    {
        protected override void OnSetup()
        {
            AllowedGroup.Set(new[] { "CommonUI" });
            base.OnSetup();
        }
    }
}
