using System;
using System.Collections.Generic;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UIDependencyResolverUtility
    {
        private static readonly List<UIDependencyResolverRegistrar> _registrars;

        static UIDependencyResolverUtility()
        {
            _registrars = new List<UIDependencyResolverRegistrar>(
                ReflectionFactory.CreateAllInstances<UIDependencyResolverRegistrar>());
        }

        public static IUIDependencyResolver GetResolver()
        {
            foreach (UIDependencyResolverRegistrar registrar in _registrars)
            {
                IUIDependencyResolver resolver = registrar.GetResolver();
                if (resolver != null)
                {
                    return resolver;
                }
            }

            return null;
        }

        public static IUIDependencyResolver GetRequiredResolver()
        {
            IUIDependencyResolver resolver = GetResolver();
            if (resolver != null)
            {
                return resolver;
            }

            throw new InvalidOperationException(
                "[UISystem] No dependency resolver registrar returned an active resolver.");
        }

        public static bool TryInstantiate(Type type, out object instance)
        {
            IUIDependencyResolver resolver = GetResolver();
            if (resolver == null)
            {
                instance = null;
                return false;
            }

            instance = resolver.Instantiate(type);
            return instance != null;
        }

        public static bool TryResolve(Type type, out object instance)
        {
            IUIDependencyResolver resolver = GetResolver();
            if (resolver == null)
            {
                instance = null;
                return false;
            }

            return resolver.TryResolve(type, out instance);
        }

        public static bool TryResolveId(Type type, string id, out object instance)
        {
            IUIDependencyResolver resolver = GetResolver();
            if (resolver == null)
            {
                instance = null;
                return false;
            }

            return resolver.TryResolveId(type, id, out instance);
        }
    }
}
