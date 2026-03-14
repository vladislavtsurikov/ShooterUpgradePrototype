using System;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class UpgradeStatRowView : BindableVisualElement
    {
        private Label _pendingDeltaLabel;
        private VisualElement _levelSegmentsContainer;
        private Label _statNameLabel;
        private Button _upgradeButton;

        public new class UxmlFactory : UxmlFactory<UpgradeStatRowView, UxmlTraits>
        {
        }

        public IObservable<Unit> OnUpgradeClicked => _upgradeButton.OnClickAsObservable();

        public void SetTitle(string text)
        {
            _statNameLabel.text = text;
        }

        public void SetPendingDelta(string text, bool visible)
        {
            _pendingDeltaLabel.text = text;
            _pendingDeltaLabel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetUpgradeEnabled(bool enabled)
        {
            _upgradeButton.SetEnabled(enabled);
        }

        public void SetLevel(int currentLevel, int maxLevel)
        {
            _levelSegmentsContainer.Clear();

            for (int index = 0; index < maxLevel; index++)
            {
                var segment = new VisualElement();
                segment.AddToClassList("level-segment");

                if (index < currentLevel)
                {
                    segment.AddToClassList("level-segment--filled");
                }

                _levelSegmentsContainer.Add(segment);
            }
        }

        protected override void InitializeElements()
        {
            _statNameLabel = this.Q<Label>("statNameLabel")
                ?? throw new InvalidOperationException("UpgradeStatRowView is missing 'statNameLabel'.");
            _pendingDeltaLabel = this.Q<Label>("pendingDeltaLabel")
                ?? throw new InvalidOperationException("UpgradeStatRowView is missing 'pendingDeltaLabel'.");
            _levelSegmentsContainer = this.Q<VisualElement>("levelSegmentsContainer")
                ?? throw new InvalidOperationException("UpgradeStatRowView is missing 'levelSegmentsContainer'.");
            _upgradeButton = this.Q<Button>("upgradeButton")
                ?? throw new InvalidOperationException("UpgradeStatRowView is missing 'upgradeButton'.");
        }
    }
}
