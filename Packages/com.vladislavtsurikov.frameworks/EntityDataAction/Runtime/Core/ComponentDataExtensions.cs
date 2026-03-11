using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public static class ComponentDataExtensions
    {
        public static Entity GetEntity(this ComponentData data)
        {
            if (data == null)
            {
                return null;
            }

            if (data.Stack is EntityDataCollection collection)
            {
                return collection.Entity;
            }

            return null;
        }
    }
}
