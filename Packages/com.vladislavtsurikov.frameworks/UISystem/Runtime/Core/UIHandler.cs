using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public abstract class UIHandler
    {
        private readonly UIChildrenModule _childrenModule;
        private readonly ViewResolver _viewResolver;
        private bool _isInitialized;

        public UIHandler Parent { get; private set; }
        public string InstanceKey { get; private set; }
        internal UIHandlerManager UIHandlerManager { get; private set; }
        internal readonly BoolReactiveProperty IsActive = new();
        public UIChildrenModule ChildrenModule => _childrenModule;
        public ViewResolver ViewResolver => _viewResolver;

        protected virtual bool AllowMultipleActiveChildren => true;

        protected internal CompositeDisposable Disposables { get; } = new();

        public static event Action<UIHandler> OnUIHandlerBeforeShow;
        public static event Action<UIHandler> OnUIHandlerOnShow;
        public static event Action<UIHandler> OnUIHandlerAfterShow;

        public static event Action<UIHandler> OnUIHandlerBeforeHide;
        public static event Action<UIHandler> OnUIHandlerHide;
        public static event Action<UIHandler> OnUIHandlerAfterHide;

        public static event Action<UIHandler> OnUIHandlerDestroyed;

        protected UIHandler()
            : this(true)
        {
        }

        protected UIHandler(bool supportsChildren)
        {
            _viewResolver = new ViewResolver(this);

            if (supportsChildren)
            {
                _childrenModule = new UIChildrenModule(this);
            }
        }

        internal UniTask Initialize(CancellationToken cancellationToken, CompositeDisposable disposables)
        {
            if (_isInitialized)
            {
                return UniTask.CompletedTask;
            }

            _isInitialized = true;
            ChildrenModule?.Initialize(AllowMultipleActiveChildren);
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
            IsActive.Value = false;
            Disposables.Dispose();
            ChildrenModule?.Dispose();

            _isInitialized = false;
            IsActive.Dispose();

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
            await UIHandlerResolver.EnsureHandlersReady();

            await BeforeShow(cancellationToken);
            await OnShow(cancellationToken);
            await AfterShow(cancellationToken);

            IsActive.Value = true;

            if (ChildrenModule != null)
            {
                await ChildrenModule.InitializeChildren(cancellationToken);
                await ChildrenModule.ShowDynamicChildren(cancellationToken);
            }
        }

        public async UniTask Hide(CancellationToken cancellationToken)
        {
            await UIHandlerResolver.EnsureHandlersReady();

            await BeforeHide(cancellationToken);
            await OnHide(cancellationToken);
            await AfterHide(cancellationToken);
            if (ChildrenModule != null)
            {
                await ChildrenModule.HideChildren(cancellationToken);
            }

            IsActive.Value = false;
        }

        internal async UniTask Destroy(bool unload, CancellationToken cancellationToken)
        {
            if (ChildrenModule != null)
            {
                await ChildrenModule.DestroyAll(unload, cancellationToken);
            }

            await DestroyUIHandler(unload, cancellationToken, Disposables);

            Dispose();

            OnUIHandlerDestroyed?.Invoke(this);
        }

        internal virtual (Type handlerType, string instanceKey) ResolveBindingContext() =>
            (GetType(), InstanceKey);

        internal void SetParent(UIHandler parent) => Parent = parent;
        internal void SetInstanceKey(string instanceKey) => InstanceKey = instanceKey;
        internal void SetUIHandlerManager(UIHandlerManager uiHandlerManager) => UIHandlerManager = uiHandlerManager;
    }
}
