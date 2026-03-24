using System;
using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class ComponentBindingUIHandler : UIHandler
    {
        private readonly UnityUIBindingAccess _bindingAccess;

        protected UIComponentBinder ComponentBinder => _bindingAccess.Binder;

        protected ComponentBindingUIHandler() =>
            _bindingAccess = new UnityUIBindingAccess(this);

        public T GetUIComponent<T>(string bindingId, Type handlerType, int index = 0) where T : MonoBehaviour =>
            _bindingAccess.GetUIComponent<T>(bindingId, handlerType, index);

        public virtual T GetUIComponent<T>(string bindingId, int index = 0) where T : MonoBehaviour =>
            _bindingAccess.GetUIComponent<T>(bindingId, index);

        protected virtual void DisposeComponentBindingUIHandler()
        {
        }

        public override void DisposeUIHandler()
        {
            _bindingAccess.Dispose();
            DisposeComponentBindingUIHandler();
        }
    }
}
