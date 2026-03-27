using System;
using Cysharp.Threading.Tasks;

namespace VladislavTsurikov.UISystem.Runtime.Core
{
    public static class UIHandlerUtility
    {
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
            string id = UIHandlerBindingId.FromParentType(parentType, instanceKey);
            UIDependencyResolver resolver = UIDependencyResolverUtility.GetResolver();
            if (resolver != null && resolver.TryResolveId(typeof(T), id, out object instance) && instance is T handler)
            {
                return handler;
            }

            return null;
        }

        public static T FindHandler<T>(Type parentType) where T : UIHandler
            => FindHandler<T>(parentType, null);
    }
}
