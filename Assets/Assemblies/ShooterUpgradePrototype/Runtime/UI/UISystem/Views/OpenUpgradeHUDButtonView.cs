using System;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class OpenUpgradeHUDButtonView : Button, IBindableUIElement
    {
        private readonly Subject<Unit> _clicked = new();
        private bool _callbackRegistered;

        public new class UxmlFactory : UxmlFactory<OpenUpgradeHUDButtonView, UxmlTraits>
        {
        }

        public string BindingId => nameof(OpenUpgradeHUDButtonView);
        public VisualElement Element => this;
        public IObservable<Unit> OnClicked
        {
            get
            {
                EnsureInitialized();
                return _clicked;
            }
        }

        private void EnsureInitialized()
        {
            if (_callbackRegistered)
            {
                return;
            }

            clicked += HandleClicked;
            _callbackRegistered = true;
        }

        private void HandleClicked() => _clicked.OnNext(Unit.Default);
    }
}
