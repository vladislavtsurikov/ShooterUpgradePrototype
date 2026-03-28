using System;
using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public sealed class ViewResolver
    {
        private readonly UIPresenter _presenter;
        private readonly DependencyResolver _resolver;

        public ViewResolver(UIPresenter presenter)
        {
            _presenter = presenter;
            _resolver = DependencyResolverProvider.GetResolver();
        }

        public TView GetView<TView>(string bindingId, Type presenterType, int index = 0)
        {
            ViewKey key = new(typeof(TView), presenterType, bindingId, index, _presenter.InstanceKey);
            return ResolveWithKey<TView>(key);
        }

        public TView GetView<TView>(string bindingId, int index = 0)
        {
            (Type presenterType, string instanceKey) = _presenter.ResolveBindingContext();
            ViewKey key = new(typeof(TView), presenterType, bindingId, index, instanceKey);
            return ResolveWithKey<TView>(key);
        }

        public bool TryGetView<TView>(string bindingId, out TView view, int index = 0)
        {
            (Type presenterType, string instanceKey) = _presenter.ResolveBindingContext();
            ViewKey key = new(typeof(TView), presenterType, bindingId, index, instanceKey);
            return TryResolveWithKey(key, out view);
        }

        private TView ResolveWithKey<TView>(ViewKey key)
        {
            if (_resolver.TryResolveId(typeof(TView), key.Id, out object instance) && instance is TView typedView)
            {
                return typedView;
            }

            throw new InvalidOperationException(
                $"[UISystem] Failed to resolve bound view `{typeof(TView).Name}` with id `{key.Id}`.");
        }

        private bool TryResolveWithKey<TView>(ViewKey key, out TView view)
        {
            if (_resolver.TryResolveId(typeof(TView), key.Id, out object instance) && instance is TView typedView)
            {
                view = typedView;
                return true;
            }

            view = default;
            return false;
        }
    }
}
