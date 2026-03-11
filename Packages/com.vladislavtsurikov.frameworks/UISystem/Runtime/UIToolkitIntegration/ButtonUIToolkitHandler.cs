#if UI_SYSTEM_ZENJECT
using System;
using UnityEngine.UIElements;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class ButtonUIToolkitHandler : ChildSpawningUIToolkitHandler
    {
        protected ButtonUIToolkitHandler(DiContainer container)
            : base(container)
        {
        }

        public override TElement GetUIComponent<TElement>(string bindingId, int index = 0)
        {
            Type handlerType = Parent?.GetType() ?? GetType();
            return ResolveWithId<TElement>(bindingId, handlerType, index);
        }
    }
}
#endif
