using System;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public sealed class UnityUIBindingAccess : IDisposable
    {
        private readonly UIHandler _handler;
        private readonly IUIDependencyResolver _resolver;

        public UnityUIBindingAccess(UIHandler handler)
        {
            _handler = handler;
            _resolver = UIDependencyResolverUtility.GetRequiredResolver();
            Binder = new UIComponentBinder(handler);
        }

        public UIComponentBinder Binder { get; }

        public T GetUIComponent<T>(string bindingId, Type handlerType, int index = 0) where T : MonoBehaviour
        {
            string id = UIBindingId.FromTypeAndIndex(handlerType, bindingId, index);
            if (_resolver.TryResolveId(typeof(T), id, out object instance) && instance is T typedInstance)
            {
                return typedInstance;
            }

            throw new InvalidOperationException(
                $"[UISystem] Failed to resolve Unity UI component `{typeof(T).Name}` with id `{id}`.");
        }

        public T GetUIComponent<T>(string bindingId, int index = 0) where T : MonoBehaviour =>
            GetUIComponent<T>(bindingId, _handler.GetType(), index);

        public void Dispose() => Binder.Dispose();
    }
}
