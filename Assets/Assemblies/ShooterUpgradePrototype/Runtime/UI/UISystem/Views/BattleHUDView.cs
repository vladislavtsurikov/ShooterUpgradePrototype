using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class BattleHUDView : VisualElement, IBindableUIElement
    {
        public new class UxmlFactory : UxmlFactory<BattleHUDView, UxmlTraits>
        {
        }

        public string BindingId => nameof(BattleHUDView);
        public VisualElement Element => this;
    }
}
