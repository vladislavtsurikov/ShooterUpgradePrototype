using System;
using UnityEngine.UIElements;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class ComponentBindingUIToolkitHandler : DiContainerUIHandler
    {
        protected readonly UIToolkitElementBinder ElementBinder;

        protected ComponentBindingUIToolkitHandler(DiContainer container)
            : base(container) =>
            ElementBinder = new UIToolkitElementBinder(container, this);

        public TElement GetUIComponent<TElement>(string bindingId, Type handlerType, int index = 0)
            where TElement : VisualElement => ResolveWithId<TElement>(bindingId, handlerType, index);

        public virtual TElement GetUIComponent<TElement>(string bindingId, int index = 0)
            where TElement : VisualElement => ResolveWithId<TElement>(bindingId, GetType(), index);

        public virtual bool TryGetUIComponent<TElement>(string bindingId, out TElement element, int index = 0)
            where TElement : VisualElement
        {
            string id = UIToolkitBindingId.FromTypeAndIndex(GetType(), bindingId, index);

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

        protected TElement ResolveWithId<TElement>(string bindingId, Type handlerType, int index)
            where TElement : VisualElement
        {
            string id = UIToolkitBindingId.FromTypeAndIndex(handlerType, bindingId, index);
            return _container.ResolveId<TElement>(id);
        }

        protected virtual void DisposeComponentBindingUIHandler()
        {
        }

        public override void DisposeUIHandler()
        {
            ElementBinder.Dispose();
            DisposeComponentBindingUIHandler();
        }
    }
}
