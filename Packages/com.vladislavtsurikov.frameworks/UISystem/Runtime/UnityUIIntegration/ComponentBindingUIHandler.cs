using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public abstract class ComponentBindingUIHandler : UIHandler
    {
        private readonly UIComponentBinder _componentBinder;
        protected UIComponentBinder ComponentBinder => _componentBinder;

        protected ComponentBindingUIHandler() =>
            _componentBinder = new UIComponentBinder(this);

        protected virtual void DisposeComponentBindingUIHandler()
        {
        }

        public override void DisposeUIHandler()
        {
            _componentBinder.Dispose();
            DisposeComponentBindingUIHandler();
        }
    }
}
