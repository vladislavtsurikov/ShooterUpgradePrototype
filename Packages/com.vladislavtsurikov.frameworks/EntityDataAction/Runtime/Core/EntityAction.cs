using OdinSerializer;
using UnityEngine;
using VladislavTsurikov.Nody.Runtime.Core;
using Action = VladislavTsurikov.ActionFlow.Runtime.Actions.Action;

namespace VladislavTsurikov.EntityDataAction.Runtime.Core
{
    public abstract class EntityAction : Action
    {
        [HideInInspector]
        public bool ShowGizmo;

        public Entity Entity
        {
            get
            {
                EntityActionCollection collection = Stack as EntityActionCollection;
                return collection?.Entity;
            }
        }

        protected T Get<T>() where T : ComponentData
        {
            return (T)Entity.Data.GetElement(typeof(T));
        }
    }
}
