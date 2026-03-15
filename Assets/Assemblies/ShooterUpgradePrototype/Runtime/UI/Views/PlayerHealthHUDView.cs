using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class PlayerHealthHUDView : Label, IBindableUIElement
    {
        public new class UxmlFactory : UxmlFactory<PlayerHealthHUDView, UxmlTraits>
        {
        }

        public string BindingId => nameof(PlayerHealthHUDView);
        public VisualElement Element => this;

        public void SetHealthText(string value) => text = value;
    }
}
