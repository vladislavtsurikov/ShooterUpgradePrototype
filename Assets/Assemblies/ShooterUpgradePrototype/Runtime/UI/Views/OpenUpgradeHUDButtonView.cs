using System;
using Cysharp.Threading.Tasks;
using UnityEngine.Localization.Settings;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class OpenUpgradeHUDButtonView : Button, IBindableUIElement
    {
        public new class UxmlFactory : UxmlFactory<OpenUpgradeHUDButtonView, UxmlTraits>
        {
        }

        private const string UITableName = "UILocalization";

        public OpenUpgradeHUDButtonView()
        {
            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                LoadTextAsync().Forget();
            });
        }

        private async UniTaskVoid LoadTextAsync()
        {
            var handle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(
                UITableName,
                "hud.open-upgrade");

            text = await handle.Task;
        }

        public string BindingId => nameof(OpenUpgradeHUDButtonView);
        public VisualElement Element => this;
        public IObservable<Unit> OnClicked => this.OnClickAsObservable();
    }
}
