using System;
using System.Collections.Generic;
using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class ViewBindingScope : IDisposable
    {
        private readonly List<ViewKey> _records = new();
        private readonly UIPresenter _presenter;

        protected UIPresenter UIPresenter => _presenter;

        protected ViewBindingScope(UIPresenter presenter)
        {
            _presenter = presenter;
        }

        protected void RegisterBindings<TNode>(
            IEnumerable<TNode> nodes,
            Func<TNode, string> getBindingId,
            Func<TNode, string> getInstanceKey = null)
            where TNode : class
        {
            var repeats = new Dictionary<(Type, string), int>();

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
                (Type, string) repeatKey = (type, rawBindingId);
                repeats.TryGetValue(repeatKey, out int index);
                repeats[repeatKey] = index + 1;

                ViewKey key = new(
                    type,
                    _presenter.GetType(),
                    rawBindingId,
                    index,
                    getInstanceKey?.Invoke(node));

                Dependencies.BindInstance(type, key.Id, node);

                _records.Add(key);
            }
        }

        public void Dispose()
        {
            foreach (ViewKey key in _records)
            {
                Dependencies.UnbindId(key.ViewType, key.Id);
            }

            _records.Clear();
        }
    }
}
