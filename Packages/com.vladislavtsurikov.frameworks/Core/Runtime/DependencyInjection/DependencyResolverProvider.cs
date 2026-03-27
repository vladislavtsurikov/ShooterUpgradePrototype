using System;
using System.Linq;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.Core.Runtime.DependencyInjection
{
    public static class DependencyResolverProvider
    {
        private static readonly DependencyResolver[] _resolvers;

        static DependencyResolverProvider()
        {
            _resolvers = ReflectionFactory.CreateAllInstances<DependencyResolver>().ToArray();
        }

        public static DependencyResolver GetResolver()
        {
            foreach (DependencyResolver resolver in _resolvers)
            {
                return resolver;
            }

            return null;
        }

        public static DependencyResolver GetRequiredResolver()
        {
            DependencyResolver resolver = GetResolver();
            if (resolver != null)
            {
                return resolver;
            }

            throw new InvalidOperationException(
                "[Core] No dependency resolver is configured.");
        }
    }
}
