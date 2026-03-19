using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace AutoStrike.MobileInputUI.MobileInputUI.Runtime
{
    public sealed class MobileControlsView : BindableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<MobileControlsView, UxmlTraits>
        {
        }
    }
}
