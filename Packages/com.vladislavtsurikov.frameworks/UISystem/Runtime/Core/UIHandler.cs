using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UniRx;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UIHandler
    {
        private ChildActivityTracker _childTracker;
        private readonly Dictionary<string, UIHandler> _dynamicChildren = new();
        private bool _isActive;
        private bool _isInitialized;
        private SingleActiveUIChildSwitcher _switcher;

        protected ReactiveCollection<UIHandler> Children { get; } = new();
        protected virtual bool AllowMultipleActiveChildren => true;

        public UIHandler Parent { get; private set; }
        public string InstanceKey { get; private set; }
        internal UIHandlerManager UIHandlerManager { get; private set; }

        public bool IsActive
        {
            get => _isActive;
            private set
            {
                if (value == _isActive)
                {
                    return;
                }

                if (value)
                {
                    BecameActive?.Invoke(this);
                }

                _isActive = value;
            }
        }

        protected internal CompositeDisposable Disposables { get; } = new();

        public event Action<UIHandler> BecameActive;

        public static event Action<UIHandler> OnUIHandlerBeforeShow;
        public static event Action<UIHandler> OnUIHandlerOnShow;
        public static event Action<UIHandler> OnUIHandlerAfterShow;

        public static event Action<UIHandler> OnUIHandlerBeforeHide;
        public static event Action<UIHandler> OnUIHandlerHide;
        public static event Action<UIHandler> OnUIHandlerAfterHide;

        public static event Action<UIHandler> OnUIHandlerDestroyed;

        internal UniTask Initialize(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_isInitialized)
            {
                return UniTask.CompletedTask;
            }

            _isInitialized = true;

            TrackChildActivation();
            InitializeUIHandler(cancellationToken, disposables);
            return UniTask.CompletedTask;
        }

        protected virtual UniTask InitializeUIHandler(CancellationToken cancellationToken,
            CompositeDisposable disposables) => UniTask.CompletedTask;

        protected virtual UniTask BeforeShowUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask OnShowUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask AfterShowUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask BeforeHideUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask OnHideUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask AfterHideUIHandler(CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        protected virtual UniTask
            DestroyUIHandler(bool unload, CancellationToken ct, CompositeDisposable disposables) =>
            UniTask.CompletedTask;

        public virtual void DisposeUIHandler()
        {
        }

        internal void Dispose()
        {
            Disposables.Dispose();
            _childTracker?.Dispose();
            _switcher?.Dispose();
            _dynamicChildren.Clear();

            IsActive = false;
            _isInitialized = false;

            DisposeUIHandler();
        }

        protected async UniTask BeforeShow(CancellationToken ct)
        {
            OnUIHandlerBeforeShow?.Invoke(this);
            await BeforeShowUIHandler(ct, Disposables);
        }

        protected async UniTask OnShow(CancellationToken ct)
        {
            OnUIHandlerOnShow?.Invoke(this);
            await OnShowUIHandler(ct, Disposables);
        }

        protected async UniTask AfterShow(CancellationToken ct)
        {
            OnUIHandlerAfterShow?.Invoke(this);
            await AfterShowUIHandler(ct, Disposables);
        }

        protected async UniTask BeforeHide(CancellationToken ct)
        {
            OnUIHandlerBeforeHide?.Invoke(this);
            await BeforeHideUIHandler(ct, Disposables);
        }

        protected async UniTask OnHide(CancellationToken ct)
        {
            OnUIHandlerHide?.Invoke(this);
            await OnHideUIHandler(ct, Disposables);
        }

        protected async UniTask AfterHide(CancellationToken ct)
        {
            OnUIHandlerAfterHide?.Invoke(this);
            await AfterHideUIHandler(ct, Disposables);
        }

        public async UniTask Show(CancellationToken cancellationToken)
        {
            await UIHandlerUtility.EnsureHandlersReady();

            await BeforeShow(cancellationToken);
            await OnShow(cancellationToken);
            await AfterShow(cancellationToken);

            IsActive = true;

            await InitializeChildren(cancellationToken);
        }

        public async UniTask Hide(CancellationToken cancellationToken)
        {
            await UIHandlerUtility.EnsureHandlersReady();

            await BeforeHide(cancellationToken);
            await OnHide(cancellationToken);
            await AfterHide(cancellationToken);
            await HideChildren(cancellationToken);

            IsActive = false;
        }

        internal async UniTask Destroy(bool unload, CancellationToken cancellationToken)
        {
            foreach (UIHandler child in Children)
            {
                await child.Destroy(unload, cancellationToken);
            }

            await DestroyUIHandler(unload, cancellationToken, Disposables);

            Dispose();

            OnUIHandlerDestroyed?.Invoke(this);
        }

        protected internal void AddUIHandlerChild(UIHandler child) => Children.Add(child);
        protected internal void RemoveUIHandlerChild(UIHandler child)
        {
            Children.Remove(child);

            if (!string.IsNullOrEmpty(child.InstanceKey))
            {
                _dynamicChildren.Remove(child.InstanceKey);
            }
        }

        internal void SetParent(UIHandler parent) => Parent = parent;
        internal void SetInstanceKey(string instanceKey) => InstanceKey = instanceKey;
        internal void SetUIHandlerManager(UIHandlerManager uiHandlerManager) => UIHandlerManager = uiHandlerManager;

        internal void RegisterDynamicChild(UIHandler child)
        {
            if (string.IsNullOrEmpty(child.InstanceKey))
            {
                throw new InvalidOperationException(
                    $"Dynamic child `{child.GetType().FullName}` must have a non-empty instance key.");
            }

            _dynamicChildren.Add(child.InstanceKey, child);
        }

        internal bool TryGetRegisteredDynamicChild(string instanceKey, out UIHandler child) =>
            _dynamicChildren.TryGetValue(instanceKey, out child);

        internal void UnregisterDynamicChild(string instanceKey)
        {
            if (string.IsNullOrEmpty(instanceKey))
            {
                return;
            }

            _dynamicChildren.Remove(instanceKey);
        }

        protected UniTask<THandler> CreateDynamicChild<THandler>(
            string instanceKey,
            bool showAutomatically = false,
            CancellationToken cancellationToken = default)
            where THandler : UIHandler
        {
            EnsureUIHandlerManager();
            return UIHandlerManager.CreateDynamicChild<THandler>(this, instanceKey, showAutomatically,
                cancellationToken);
        }

        protected THandler GetDynamicChild<THandler>(string instanceKey)
            where THandler : UIHandler
        {
            EnsureUIHandlerManager();
            return UIHandlerManager.GetDynamicChild<THandler>(this, instanceKey);
        }

        protected bool TryGetDynamicChild<THandler>(string instanceKey, out THandler handler)
            where THandler : UIHandler
        {
            EnsureUIHandlerManager();
            return UIHandlerManager.TryGetDynamicChild(this, instanceKey, out handler);
        }

        protected UniTask DestroyDynamicChild<THandler>(
            string instanceKey,
            bool unload,
            CancellationToken cancellationToken = default)
            where THandler : UIHandler
        {
            EnsureUIHandlerManager();
            return UIHandlerManager.DestroyDynamicChild<THandler>(this, instanceKey, unload, cancellationToken);
        }

        private void TrackChildActivation()
        {
            _childTracker?.Dispose();
            _childTracker = new ChildActivityTracker(Children);

            if (!AllowMultipleActiveChildren)
            {
                _switcher?.Dispose();
                _switcher = new SingleActiveUIChildSwitcher(_childTracker);
            }
        }

        private async Task InitializeChildren(CancellationToken cancellationToken)
        {
            foreach (UIHandler child in Children)
            {
                await child.Initialize(cancellationToken, child.Disposables);
            }
        }

        private async Task HideChildren(CancellationToken cancellationToken)
        {
            List<UIHandler> childrenToHide = new();

            foreach (UIHandler child in Children)
            {
                if (child.IsActive)
                {
                    childrenToHide.Add(child);
                }
            }

            foreach (UIHandler child in childrenToHide)
            {
                await child.Hide(cancellationToken);
            }
        }

        private void EnsureUIHandlerManager()
        {
            if (UIHandlerManager == null)
            {
                throw new InvalidOperationException(
                    $"UIHandlerManager is not assigned for `{GetType().FullName}`.");
            }
        }
    }
}
