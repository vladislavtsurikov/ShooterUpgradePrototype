using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public abstract class EntityComponentData : ComponentData
    {
        public Entity Entity
        {
            get
            {
                EntityDataCollection collection = Stack as EntityDataCollection;
                return collection?.Entity;
            }
        }
    }
}
