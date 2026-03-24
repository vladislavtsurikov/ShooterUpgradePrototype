using System;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public sealed class UIToolkitBindingAccess : IDisposable
    {
        private readonly UIToolkitUIHandler _handler;
        private readonly IUIToolkitBindingContextResolver _bindingContextResolver;
        private readonly IUIDependencyResolver _resolver;

        public UIToolkitBindingAccess(
            UIToolkitUIHandler handler,
            IUIToolkitBindingContextResolver bindingContextResolver)
        {
            _handler = handler;
            _bindingContextResolver = bindingContextResolver ?? SelfUIToolkitBindingContextResolver.Instance;
            _resolver = UIDependencyResolverUtility.GetRequiredResolver();
            Binder = new UIToolkitElementBinder(handler);
        }

        public UIToolkitElementBinder Binder { get; }

        public TElement GetView<TElement>(string bindingId, Type handlerType, int index = 0)
            where TElement : VisualElement =>
            ResolveWithId<TElement>(bindingId, handlerType, _handler.InstanceKey, index);

        public TElement GetView<TElement>(string bindingId, int index = 0)
            where TElement : VisualElement
        {
            (Type handlerType, string instanceKey) = ResolveCurrentBindingContext();
            return ResolveWithId<TElement>(bindingId, handlerType, instanceKey, index);
        }

        public bool TryGetUIComponent<TElement>(string bindingId, out TElement element, int index = 0)
            where TElement : VisualElement
        {
            (Type handlerType, string instanceKey) = ResolveCurrentBindingContext();
            string id = UIBindingId.FromTypeAndIndex(handlerType, bindingId, index, instanceKey);

            if (_resolver.TryResolveId(typeof(TElement), id, out object instance) && instance is TElement typedElement)
            {
                element = typedElement;
                return true;
            }

            element = null;
            return false;
        }

        public (Type handlerType, string instanceKey) ResolveCurrentBindingContext() =>
            (_bindingContextResolver.ResolveHandlerType(_handler), _bindingContextResolver.ResolveInstanceKey(_handler));

        public void Dispose() => Binder.Dispose();

        private TElement ResolveWithId<TElement>(string bindingId, Type handlerType, string instanceKey, int index)
            where TElement : VisualElement
        {
            string id = UIBindingId.FromTypeAndIndex(handlerType, bindingId, index, instanceKey);
            if (_resolver.TryResolveId(typeof(TElement), id, out object instance) && instance is TElement typedElement)
            {
                return typedElement;
            }

            throw new InvalidOperationException(
                $"[UISystem] Failed to resolve UI Toolkit element `{typeof(TElement).Name}` with id `{id}`.");
        }
    }
}
