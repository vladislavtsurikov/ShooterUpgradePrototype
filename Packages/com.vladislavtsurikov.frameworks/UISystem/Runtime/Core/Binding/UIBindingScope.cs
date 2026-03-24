using System;
using System.Collections.Generic;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UIBindingScope
    {
        private readonly IUIDependencyResolver _resolver;
        private readonly List<BoundBindingRecord> _records = new();
        private readonly BindingRepeatTracker _repeatTracker = new();
        private readonly UIHandler _uiHandler;

        protected UIBindingScope(UIHandler handler)
        {
            _resolver = UIDependencyResolverUtility.GetRequiredResolver();
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
                string finalId = UIBindingId.FromTypeAndIndex(
                    _uiHandler.GetType(),
                    rawBindingId,
                    index,
                    getInstanceKey?.Invoke(node));

                _resolver.BindInstance(type, finalId, node);

                _records.Add(new BoundBindingRecord(type, finalId));
            }
        }

        public void Dispose()
        {
            foreach (BoundBindingRecord record in _records)
            {
                _resolver.UnbindId(record.Type, record.Id);
            }

            _records.Clear();
            _repeatTracker.Reset();
        }
    }
}
