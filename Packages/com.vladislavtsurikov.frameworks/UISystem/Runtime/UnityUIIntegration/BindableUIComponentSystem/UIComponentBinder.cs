using UnityEngine;
using VladislavTsurikov.UISystem.Runtime.Core;
namespace VladislavTsurikov.UISystem.Runtime.UnityUIIntegration
{
    public class UIComponentBinder : ViewBindingScope
    {
        public UIComponentBinder(UIPresenter presenter)
            : base(presenter)
        {
        }

        public void BindUIComponentsFrom(GameObject instance)
        {
            IBindableView[] allComponents = instance.GetComponentsInChildren<IBindableView>(true);
            RegisterBindings(allComponents, component => component.BindingId);
        }
    }
}
