using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UIBackSystem.Runtime
{
    public static class UIBackStack
    {
        private static readonly Stack<UIPresenter> _stack = new();

        static UIBackStack()
        {
            UIPresenter.OnUIPresenterOnShow += Push;
            UIPresenter.OnUIPresenterHide += _ => Peek();
            UIPresenter.OnUIPresenterDestroyed += Remove;
        }

        private static void Push(UIPresenter handler)
        {
            if (!_stack.Contains(handler))
            {
                _stack.Push(handler);
            }
        }

        internal static UIPresenter Peek() => _stack.Count > 0 ? _stack.Peek() : null;

        internal static async UniTask PopAndHide(CancellationToken cancellationToken)
        {
            if (_stack.Count > 0)
            {
                UIPresenter top = _stack.Pop();
                await top.Hide(cancellationToken);
            }
        }

        private static void Remove(UIPresenter handler)
        {
            if (!_stack.Contains(handler))
            {
                return;
            }

            UIPresenter[] reordered = _stack
                .Where(item => !ReferenceEquals(item, handler))
                .Reverse()
                .ToArray();

            _stack.Clear();

            foreach (UIPresenter item in reordered)
            {
                _stack.Push(item);
            }
        }
    }
}
