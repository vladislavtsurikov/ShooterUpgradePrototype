using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class PlayerHealthHUDView : Label, IBindableUIElement
    {
        private const string UITableName = "UILocalization";

        public new class UxmlFactory : UxmlFactory<PlayerHealthHUDView, UxmlTraits>
        {
        }

        public string BindingId => nameof(PlayerHealthHUDView);
        public VisualElement Element => this;

        public void SetHealthText(float currentValue, float maxValue)
        {
            text = LocalizationSettings.StringDatabase.GetLocalizedString(
                UITableName,
                "hud.health",
                new object[] { currentValue, maxValue });
        }
    }
}
