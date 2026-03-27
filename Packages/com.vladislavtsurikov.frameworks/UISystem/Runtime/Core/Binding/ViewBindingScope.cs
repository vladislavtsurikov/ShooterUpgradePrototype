using System;
using System.Collections.Generic;
using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class ViewBindingScope : IDisposable
    {
        private readonly DependencyResolver _resolver;
        private readonly List<ViewKey> _records = new();
        private readonly ViewBindingRepeatTracker _repeatTracker = new();
        private readonly UIHandler _uiHandler;

        protected ViewBindingScope(UIHandler handler)
        {
            _resolver = DependencyResolverProvider.GetResolver();
            _uiHandler = handler;
        }

        protected UIHandler UIHandler => _uiHandler;

        protected void RegisterBindings<TNode>(
            IEnumerable<TNode> nodes,
            Func<TNode, string> getBindingId,
            Func<TNode, string> getInstanceKey = null)
            where TNode : class
        {
            foreach (TNode node in nodes)
            {
                if (node == null)
                {
                    continue;
                }

                string rawBindingId = getBindingId(node);
                if (string.IsNullOrEmpty(rawBindingId))
                {
                    continue;
                }

                Type type = node.GetType();
                int index = _repeatTracker.GetAndIncrement(type, rawBindingId);
                ViewKey key = new(
                    type,
                    _uiHandler.GetType(),
                    rawBindingId,
                    index,
                    getInstanceKey?.Invoke(node));

                _resolver.BindInstance(type, key.Id, node);

                _records.Add(key);
            }
        }

        public void Dispose()
        {
            foreach (ViewKey key in _records)
            {
                _resolver.UnbindId(key.ViewType, key.Id);
            }

            _records.Clear();
            _repeatTracker.Reset();
        }
    }
}
