using System;
using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public sealed class ViewResolver
    {
        private readonly UIHandler _handler;
        private readonly DependencyResolver _resolver;

        public ViewResolver(UIHandler handler)
        {
            _handler = handler;
            _resolver = DependencyResolverProvider.GetResolver();
        }

        public TView GetView<TView>(string bindingId, Type handlerType, int index = 0)
        {
            return ResolveWithId<TView>(bindingId, handlerType, _handler.InstanceKey, index);
        }

        public TView GetView<TView>(string bindingId, int index = 0)
        {
            (Type handlerType, string instanceKey) = _handler.ResolveBindingContext();
            return ResolveWithId<TView>(bindingId, handlerType, instanceKey, index);
        }

        public bool TryGetView<TView>(string bindingId, out TView view, int index = 0)
        {
            (Type handlerType, string instanceKey) = _handler.ResolveBindingContext();
            return TryResolveWithId(bindingId, handlerType, instanceKey, index, out view);
        }

        private TView ResolveWithId<TView>(string bindingId, Type handlerType, string instanceKey, int index)
        {
            string id = ViewBindingId.FromTypeAndIndex(handlerType, bindingId, index, instanceKey);

            if (_resolver.TryResolveId(typeof(TView), id, out object instance) && instance is TView typedView)
            {
                return typedView;
            }

            throw new InvalidOperationException(
                $"[UISystem] Failed to resolve bound view `{typeof(TView).Name}` with id `{id}`.");
        }

        private bool TryResolveWithId<TView>(
            string bindingId,
            Type handlerType,
            string instanceKey,
            int index,
            out TView view)
        {
            string id = ViewBindingId.FromTypeAndIndex(handlerType, bindingId, index, instanceKey);

            if (_resolver.TryResolveId(typeof(TView), id, out object instance) && instance is TView typedView)
            {
                view = typedView;
                return true;
            }

            view = default;
            return false;
        }
    }
}
