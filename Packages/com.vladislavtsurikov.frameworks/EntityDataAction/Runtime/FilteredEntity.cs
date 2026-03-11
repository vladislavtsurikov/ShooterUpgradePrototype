using Cysharp.Threading.Tasks;
using VladislavTsurikov.EntityDataAction.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime
{
    public abstract class FilteredEntity : EntityMonoBehaviour
    {
        public virtual string[] GetAllowedActionGroups() => null;

        protected override void BeforeOnSetupEntity()
        {
            Actions.SetAllowedGroupAttributes(GetAllowedActionGroups());
        }
    }
}
