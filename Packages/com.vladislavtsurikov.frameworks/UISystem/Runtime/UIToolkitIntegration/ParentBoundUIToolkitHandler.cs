#if UI_SYSTEM_ZENJECT
using System;
using UnityEngine.UIElements;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class ParentBoundUIToolkitHandler : ChildSpawningUIToolkitHandler
    {
        protected ParentBoundUIToolkitHandler(DiContainer container)
            : base(container)
        {
        }

        public override TElement GetView<TElement>(string bindingId, int index = 0)
        {
            Type handlerType = Parent?.GetType() ?? GetType();
            string instanceKey = Parent?.InstanceKey;
            return ResolveWithId<TElement>(bindingId, handlerType, instanceKey, index);
        }

        public override bool TryGetUIComponent<TElement>(string bindingId, out TElement element, int index = 0)
        {
            Type handlerType = Parent?.GetType() ?? GetType();
            string instanceKey = Parent?.InstanceKey;
            string id = UIToolkitBindingId.FromTypeAndIndex(handlerType, bindingId, index, instanceKey);

            try
            {
                element = _container.ResolveId<TElement>(id);
                return element != null;
            }
            catch
            {
                element = null;
                return false;
            }
        }
    }
}
#endif
