using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.Core;
namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public class UIComponentBinder : UIBindingScope
    {
        public UIComponentBinder(UIHandler handler)
            : base(handler)
        {
        }

        public void BindUIComponentsFrom(GameObject instance)
        {
            IBindableUI[] allComponents = instance.GetComponentsInChildren<IBindableUI>(true);
            RegisterBindings(allComponents, component => component.BindingId);
        }
    }
}
