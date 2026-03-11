using System;
using Nody.Runtime.Core;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.EntityDataAction.Runtime.Core;
using VladislavTsurikov.ReflectionUtility;

namespace VladislavTsurikov.EntityDataAction.Shared.Runtime.Style
{
    public sealed class StyleActionCollection : EntityActionCollection
    {
        protected override bool AllowCreateNodeStack(Type type)
        {
            if (!typeof(EntityAction).IsAssignableFrom(type))
            {
                return false;
            }
            
            var groups = type.GetAttributes<GroupAttribute>();
            if (groups == null)
            {
                return false;
            }

            foreach (GroupAttribute group in groups)
            {
                if (group != null && string.Equals(group.Name, "Style", StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
