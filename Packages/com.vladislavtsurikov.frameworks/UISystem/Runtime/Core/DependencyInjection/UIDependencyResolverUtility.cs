using System;
using System.Linq;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UIDependencyResolverUtility
    {
        private static readonly UIDependencyResolver[] _resolvers;

        static UIDependencyResolverUtility()
        {
            _resolvers = ReflectionFactory.CreateAllInstances<UIDependencyResolver>().ToArray();
        }

        public static UIDependencyResolver GetResolver()
        {
            foreach (UIDependencyResolver resolver in _resolvers)
            {
                return resolver;
            }

            return null;
        }
    }
}
