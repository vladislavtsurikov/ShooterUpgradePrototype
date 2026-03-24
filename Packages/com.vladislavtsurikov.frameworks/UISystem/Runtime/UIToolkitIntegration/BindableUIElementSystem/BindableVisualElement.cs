using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.Core;

namespace VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration
{
    public abstract class BindableVisualElement : VisualElement, IBindableUI
    {
        protected BindableVisualElement() => RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);

        public string BindingId => GetType().Name;

        protected virtual void InitializeElements()
        {
        }

        private void HandleAttachToPanel(AttachToPanelEvent evt)
        {
            InitializeElements();
            UnregisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
        }
    }
}
