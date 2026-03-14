using System;
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

        public string BindingId => nameof(OpenUpgradeHUDButtonView);
        public VisualElement Element => this;
        public IObservable<Unit> OnClicked => this.OnClickAsObservable();
    }
}
