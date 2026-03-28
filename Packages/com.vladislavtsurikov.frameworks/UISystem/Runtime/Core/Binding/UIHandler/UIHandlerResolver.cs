using System;
using Cysharp.Threading.Tasks;

using VladislavTsurikov.Core.Runtime.DependencyInjection;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UIHandlerResolver
    {
        private static readonly DependencyResolver _resolver;

        static UIHandlerResolver()
        {
            _resolver = DependencyResolverProvider.GetResolver();
        }

        public static async UniTask EnsureHandlersReady()
        {
            if (UIHandlerManager.CurrentAddFilterTask.Status == UniTaskStatus.Pending)
            {
                await UIHandlerManager.CurrentAddFilterTask;
            }
        }

        public static T FindHandler<T>() where T : UIHandler => FindHandler<T>(null);
        public static T FindHandler<T>(Type parentType, string instanceKey) where T : UIHandler
        {
            UIHandlerKey key = UIHandlerKey.FromParentType(parentType, instanceKey);
            if (TryResolve(key, out T handler))
            {
                return handler;
            }

            return null;
        }

        public static T FindHandler<T>(Type parentType) where T : UIHandler
            => FindHandler<T>(parentType, null);

        internal static bool TryResolve<T>(UIHandlerKey key, out T handler) where T : UIHandler
        {
            if (_resolver.TryResolveId(typeof(T), key.Id, out object instance) && instance is T typedHandler)
            {
                handler = typedHandler;
                return true;
            }

            handler = null;
            return false;
        }
    }
}
