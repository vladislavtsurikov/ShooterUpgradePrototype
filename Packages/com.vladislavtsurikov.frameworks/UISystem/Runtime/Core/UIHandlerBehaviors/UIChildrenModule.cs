using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public sealed class UIChildrenModule : IDisposable
    {
        private readonly UIHandler _owner;
        private readonly SerialDisposable _childActivitySubscriptions = new();
        private readonly SerialDisposable _childrenChanges = new();
        private UIHandler _activeChild;

        public ReactiveCollection<UIHandler> All { get; } = new();

        public UIChildrenModule(UIHandler owner)
        {
            _owner = owner;
        }

        public void Initialize(bool allowMultipleActiveChildren)
        {
            _activeChild = null;
            _childrenChanges.Disposable = null;
            _childActivitySubscriptions.Disposable = null;

            if (allowMultipleActiveChildren)
            {
                return;
            }

            _childrenChanges.Disposable = new CompositeDisposable(
                All.ObserveAdd().Subscribe(_ => RebuildChildActivitySubscriptions()),
                All.ObserveRemove().Subscribe(_ => RebuildChildActivitySubscriptions()),
                All.ObserveReset().Subscribe(_ => RebuildChildActivitySubscriptions()));

            RebuildChildActivitySubscriptions();
        }

        public void Add(UIHandler child) => All.Add(child);

        public void Remove(UIHandler child) => All.Remove(child);

        public UniTask<THandler> CreateDynamicChild<THandler>(
            string instanceKey,
            bool showAutomatically = false,
            CancellationToken cancellationToken = default)
            where THandler : UIHandler
        {
            return _owner.UIHandlerManager.CreateDynamicChild<THandler>(_owner, instanceKey, showAutomatically,
                cancellationToken);
        }

        public THandler GetDynamicChild<THandler>(string instanceKey)
            where THandler : UIHandler
        {
            return _owner.UIHandlerManager.GetDynamicChild<THandler>(_owner, instanceKey);
        }

        public bool TryGetDynamicChild<THandler>(string instanceKey, out THandler handler)
            where THandler : UIHandler
        {
            return _owner.UIHandlerManager.TryGetDynamicChild(_owner, instanceKey, out handler);
        }

        public UniTask DestroyChild<THandler>(
            string instanceKey,
            bool unload,
            CancellationToken cancellationToken = default)
            where THandler : UIHandler
        {
            return _owner.UIHandlerManager.DestroyDynamicChild<THandler>(_owner, instanceKey, unload,
                cancellationToken);
        }

        public async UniTask DestroyAll(bool unload, CancellationToken cancellationToken)
        {
            foreach (UIHandler child in All)
            {
                await child.Destroy(unload, cancellationToken);
            }
        }

        public async Task InitializeChildren(CancellationToken cancellationToken)
        {
            foreach (UIHandler child in All)
            {
                if (IsDynamicChild(child))
                {
                    continue;
                }

                await child.Initialize(cancellationToken, child.Disposables);
            }
        }

        public async Task HideChildren(CancellationToken cancellationToken)
        {
            List<UIHandler> childrenToHide = new();

            foreach (UIHandler child in All)
            {
                if (child.IsActive.Value)
                {
                    childrenToHide.Add(child);
                }
            }

            foreach (UIHandler child in childrenToHide)
            {
                await child.Hide(cancellationToken);
            }
        }

        public async Task ShowDynamicChildren(CancellationToken cancellationToken)
        {
            foreach (UIHandler child in All)
            {
                if (IsDynamicChild(child))
                {
                    await child.Show(cancellationToken);
                }
            }
        }

        public void Dispose()
        {
            _childrenChanges.Dispose();
            _childActivitySubscriptions.Dispose();
            All.Clear();
        }

        private static bool IsDynamicChild(UIHandler child) =>
            child != null && Attribute.IsDefined(child.GetType(), typeof(DynamicUIChildAttribute), inherit: true);

        private void RebuildChildActivitySubscriptions()
        {
            var subscriptions = new CompositeDisposable();

            foreach (UIHandler child in All)
            {
                if (child == null)
                {
                    continue;
                }

                child.IsActive
                    .Where(isActive => isActive)
                    .Subscribe(_ => OnChildBecameActive(child))
                    .AddTo(subscriptions);
            }

            _childActivitySubscriptions.Disposable = subscriptions;
        }

        private void OnChildBecameActive(UIHandler next)
        {
            if (_activeChild != null && _activeChild != next)
            {
                _activeChild.Hide(CancellationToken.None).Forget();
            }

            _activeChild = next;
        }
    }
}
