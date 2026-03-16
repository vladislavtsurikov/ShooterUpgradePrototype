using System;
using UnityEngine.Localization.Settings;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class UpgradeWindowView : BindableVisualElement
    {
        private const string UITableName = "UILocalization";

        public new class UxmlFactory : UxmlFactory<UpgradeWindowView, UxmlTraits>
        {
        }

        private Button _applyButton;
        private Button _closeButton;
        private Label _availablePointsLabel;
        private VisualElement _backdrop;
        private VisualElement _modalPanel;
        private VisualElement _rowsContainer;
        private Label _titleLabel;

        public IObservable<Unit> OnApplyClicked => _applyButton.OnClickAsObservable();
        public IObservable<Unit> OnCloseClicked => _closeButton.OnClickAsObservable();
        public IObservable<Unit> OnBackdropClicked => _backdrop
            .OnClickEventAsObservable()
            .Where(evt => ReferenceEquals(evt.target, _backdrop))
            .Select(_ => Unit.Default);

        public void SetAvailablePoints(int availablePoints)
        {
            _availablePointsLabel.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                UITableName,
                "upgrade.window.available-points",
                new object[] { availablePoints });
        }

        public void SetApplyEnabled(bool enabled)
        {
            _applyButton.SetEnabled(enabled);
        }

        protected override void InitializeElements()
        {
            _backdrop = this.Q<VisualElement>("backdrop");
            _modalPanel = this.Q<VisualElement>("modalPanel");
            _titleLabel = this.Q<Label>("titleLabel");
            _availablePointsLabel = this.Q<Label>("availablePointsLabel");
            _rowsContainer = this.Q<VisualElement>("rowsContainer");
            _applyButton = this.Q<Button>("applyButton");
            _closeButton = this.Q<Button>("closeButton");

            _titleLabel.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                UITableName,
                "upgrade.window.title");
            _applyButton.text = LocalizationSettings.StringDatabase.GetLocalizedString(
                UITableName,
                "upgrade.window.apply");
            SetAvailablePoints(0);

            _modalPanel.RegisterCallback<ClickEvent>(HandleModalClicked);
        }

        private static void HandleModalClicked(ClickEvent evt) => evt.StopPropagation();
    }
}
