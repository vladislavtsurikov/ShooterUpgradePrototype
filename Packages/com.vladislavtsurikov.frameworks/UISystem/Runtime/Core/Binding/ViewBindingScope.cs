using System;
using System.Collections.Generic;
using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class ViewBindingScope : IDisposable
    {
        private readonly DependencyResolver _resolver;
        private readonly List<BoundViewRecord> _records = new();
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
                string finalId = ViewBindingId.FromTypeAndIndex(
                    _uiHandler.GetType(),
                    rawBindingId,
                    index,
                    getInstanceKey?.Invoke(node));

                _resolver.BindInstance(type, finalId, node);

                _records.Add(new BoundViewRecord(type, finalId));
            }
        }

        public void Dispose()
        {
            foreach (BoundViewRecord record in _records)
            {
                _resolver.UnbindId(record.Type, record.Id);
            }

            _records.Clear();
            _repeatTracker.Reset();
        }
    }
}
