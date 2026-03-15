using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime
{
    public abstract class DiContainerUIHandler : UIHandler
    {
        private string _dynamicBindingId;
        protected readonly DiContainer _container;

        protected DiContainerUIHandler(DiContainer container) => _container = container;

        protected async UniTask<THandler> CreateDynamicChild<THandler>(
            string instanceKey,
            CancellationToken cancellationToken = default)
            where THandler : DiContainerUIHandler
        {
            await UIHandlerUtility.EnsureHandlersReady();
            ValidateDynamicChildType(typeof(THandler));

            if (TryGetDynamicChild(instanceKey, out THandler existingHandler))
            {
                return existingHandler;
            }

            THandler handler = _container.Instantiate<THandler>();
            string bindingId = UIHandlerBindingId.FromDynamicParent(this, instanceKey);

            AddUIHandlerChild(handler);
            handler.SetParent(this);
            handler.SetInstanceKey(instanceKey);
            handler.BindDynamicInstance(bindingId);
            await handler.Initialize(cancellationToken, handler.Disposables);

            return handler;
        }

        protected bool TryGetDynamicChild<THandler>(string instanceKey, out THandler handler)
            where THandler : DiContainerUIHandler
        {
            string bindingId = UIHandlerBindingId.FromDynamicParent(this, instanceKey);

            try
            {
                handler = _container.ResolveId<THandler>(bindingId);
                return true;
            }
            catch
            {
                handler = null;
                return false;
            }
        }

        protected async UniTask DestroyDynamicChild<THandler>(
            string instanceKey,
            bool unload,
            CancellationToken cancellationToken = default)
            where THandler : DiContainerUIHandler
        {
            if (!TryGetDynamicChild(instanceKey, out THandler handler))
            {
                return;
            }

            await handler.Destroy(unload, cancellationToken);
            RemoveUIHandlerChild(handler);
        }

        public override void DisposeUIHandler()
        {
            if (!string.IsNullOrEmpty(_dynamicBindingId))
            {
                _container.UnbindId(GetType(), _dynamicBindingId);
                _dynamicBindingId = null;
            }

            base.DisposeUIHandler();
        }

        private static void ValidateDynamicChildType(Type type)
        {
            if (!Attribute.IsDefined(type, typeof(DynamicUIChildAttribute), inherit: true))
            {
                throw new InvalidOperationException(
                    $"Dynamic child handler `{type.FullName}` must be marked with [{nameof(DynamicUIChildAttribute)}].");
            }
        }

        private void BindDynamicInstance(string bindingId)
        {
            _dynamicBindingId = bindingId;
            _container.Bind(GetType()).WithId(bindingId).FromInstance(this).AsCached();
        }
    }
}
