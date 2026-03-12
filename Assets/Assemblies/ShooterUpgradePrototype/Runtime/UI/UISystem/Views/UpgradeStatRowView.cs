using System;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class UpgradeStatRowView : VisualElement, IBindableUIElement
    {
        private Label _pendingDeltaLabel;
        private VisualElement _levelSegmentsContainer;
        private Label _statNameLabel;
        private Button _upgradeButton;
        private Action _upgradeRequested;
        private bool _initialized;

        public new class UxmlFactory : UxmlFactory<UpgradeStatRowView, UxmlTraits>
        {
        }

        public string BindingId => nameof(UpgradeStatRowView);
        public VisualElement Element => this;

        public void SetTitle(string text)
        {
            EnsureInitialized();
            _statNameLabel.text = text;
        }

        public void SetPendingDelta(string text, bool visible)
        {
            EnsureInitialized();
            _pendingDeltaLabel.text = text;
            _pendingDeltaLabel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void SetUpgradeEnabled(bool enabled)
        {
            EnsureInitialized();
            _upgradeButton.SetEnabled(enabled);
        }

        public void SetLevel(int currentLevel, int maxLevel)
        {
            EnsureInitialized();
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

        public void SetUpgradeRequestedHandler(Action handler)
        {
            EnsureInitialized();
            _upgradeRequested = handler;
        }

        private void EnsureInitialized()
        {
            if (_initialized)
            {
                return;
            }

            _statNameLabel = this.Q<Label>("statNameLabel");
            _pendingDeltaLabel = this.Q<Label>("pendingDeltaLabel");
            _levelSegmentsContainer = this.Q<VisualElement>("levelSegmentsContainer");
            _upgradeButton = this.Q<Button>("upgradeButton");

            if (_statNameLabel == null || _pendingDeltaLabel == null ||
                _levelSegmentsContainer == null || _upgradeButton == null)
            {
                throw new InvalidOperationException("UpgradeStatRowView is missing required UI elements.");
            }

            _upgradeButton.clicked += () => _upgradeRequested?.Invoke();
            _initialized = true;
        }
    }
}
