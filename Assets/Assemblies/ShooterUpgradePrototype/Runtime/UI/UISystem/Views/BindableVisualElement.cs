using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public abstract class BindableVisualElement : VisualElement, IBindableUIElement
    {
        protected BindableVisualElement() => RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);

        public string BindingId => GetType().Name;
        public VisualElement Element => this;

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
