#if UI_SYSTEM_ZENJECT
using System;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;
using Zenject;

namespace ShooterUpgradePrototype.UI.UISystem
{
    // Presenter over an already loaded parent layout.
    public abstract class ParentBoundUIToolkitHandler : ComponentBindingUIToolkitHandler
    {
        protected ParentBoundUIToolkitHandler(DiContainer container)
            : base(container)
        {
        }

        public override TElement GetUIComponent<TElement>(string bindingId, int index = 0)
        {
            Type handlerType = Parent?.GetType() ?? GetType();
            return ResolveWithId<TElement>(bindingId, handlerType, index);
        }

        public override bool TryGetUIComponent<TElement>(string bindingId, out TElement element, int index = 0)
        {
            Type handlerType = Parent?.GetType() ?? GetType();
            string id = UIToolkitBindingId.FromTypeAndIndex(handlerType, bindingId, index);

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
