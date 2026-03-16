using System;
using UnityEngine.Localization.Settings;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class OpenUpgradeHUDButtonView : Button, IBindableUIElement
    {
        private const string UITableName = "UILocalization";

        public OpenUpgradeHUDButtonView()
        {
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                text = LocalizationSettings.StringDatabase.GetLocalizedString(
                    UITableName,
                    "hud.open-upgrade");
            });
        }

        public new class UxmlFactory : UxmlFactory<OpenUpgradeHUDButtonView, UxmlTraits>
        {
        }

        public string BindingId => nameof(OpenUpgradeHUDButtonView);
        public VisualElement Element => this;
        public IObservable<Unit> OnClicked => this.OnClickAsObservable();
    }
}
