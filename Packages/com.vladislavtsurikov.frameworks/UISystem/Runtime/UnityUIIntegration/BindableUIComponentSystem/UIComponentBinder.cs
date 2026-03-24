using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.Core;
using Zenject;

namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public class UIComponentBinder : UIBindingScope
    {
        public UIComponentBinder(DiContainer container, UIHandler handler)
            : base(container, handler)
        {
        }

        public void BindUIComponentsFrom(GameObject instance)
        {
            IBindableUI[] allComponents = instance.GetComponentsInChildren<IBindableUI>(true);
            RegisterBindings(allComponents, component => component.BindingId);
        }
    }
}
