using System;
using VladislavTsurikov.ActionFlow.Runtime;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public class EntityActionCollection : ActionCollection
    {
        [field: NonSerialized]
        public Entity Entity { get; internal set; }
    }
}
