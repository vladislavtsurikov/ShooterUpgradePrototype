#if UI_SYSTEM_ZENJECT
using System;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public sealed class ZenjectUIDependencyResolver : IUIDependencyResolver
    {
        private readonly DiContainer _fallbackContainer;
        private readonly DiContainer _primaryContainer;

        public ZenjectUIDependencyResolver(DiContainer primaryContainer, DiContainer fallbackContainer = null)
        {
            _primaryContainer = primaryContainer;
            _fallbackContainer = fallbackContainer;
        }

        public object Instantiate(Type type)
        {
            if (_primaryContainer != null)
            {
                try
                {
                    return _primaryContainer.Instantiate(type);
                }
                catch
                {
                }
            }

            return _fallbackContainer?.Instantiate(type);
        }

        public void BindInstance(Type type, string id, object instance)
        {
            DiContainer container = _primaryContainer ?? _fallbackContainer;
            if (container == null)
            {
                throw new InvalidOperationException(
                    "[UISystem] Cannot bind instance because no Zenject container is available.");
            }

            container.Bind(type).WithId(id).FromInstance(instance).AsCached();
        }

        public bool TryResolve(Type type, out object instance)
        {
            if (TryResolveFromContainer(_primaryContainer, type, out instance))
            {
                return true;
            }

            return TryResolveFromContainer(_fallbackContainer, type, out instance);
        }

        public bool TryResolveId(Type type, string id, out object instance)
        {
            if (TryResolveIdFromContainer(_primaryContainer, type, id, out instance))
            {
                return true;
            }

            return TryResolveIdFromContainer(_fallbackContainer, type, id, out instance);
        }

        public void UnbindId(Type type, string id)
        {
            UnbindIdFromContainer(_primaryContainer, type, id);

            if (!ReferenceEquals(_primaryContainer, _fallbackContainer))
            {
                UnbindIdFromContainer(_fallbackContainer, type, id);
            }
        }

        private static bool TryResolveFromContainer(DiContainer container, Type type, out object instance)
        {
            if (container == null)
            {
                instance = null;
                return false;
            }

            try
            {
                instance = container.Resolve(type);
                return instance != null;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        private static bool TryResolveIdFromContainer(DiContainer container, Type type, string id, out object instance)
        {
            if (container == null)
            {
                instance = null;
                return false;
            }

            try
            {
                instance = container.ResolveId(type, id);
                return instance != null;
            }
            catch
            {
                instance = null;
                return false;
            }
        }

        private static void UnbindIdFromContainer(DiContainer container, Type type, string id)
        {
            if (container == null)
            {
                return;
            }

            try
            {
                container.UnbindId(type, id);
            }
            catch
            {
            }
        }
    }
}
#endif
