using System;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public sealed class EntityDataCollection : NodeStackOnlyDifferentTypes<ComponentData>
    {
        [field: NonSerialized]
        public Entity Entity { get; internal set; }
    }
}
