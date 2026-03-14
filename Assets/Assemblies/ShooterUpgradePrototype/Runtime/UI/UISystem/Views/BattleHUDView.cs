using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class BattleHUDView : BindableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<BattleHUDView, UxmlTraits>
        {
        }
    }
}
