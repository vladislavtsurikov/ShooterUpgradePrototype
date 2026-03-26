using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;
using VladislavTsurikov.UIElementsUtility.Runtime.Utility;
using VladislavTsurikov.UISystem.Runtime.Core;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    public sealed class OpenUpgradeHUDButtonView : Button, IBindableUI
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
        public IObservable<Unit> OnClicked => this.OnClickAsObservable();
    }
}
