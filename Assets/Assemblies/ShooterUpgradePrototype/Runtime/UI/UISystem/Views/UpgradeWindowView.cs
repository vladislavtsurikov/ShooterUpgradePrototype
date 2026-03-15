using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class UpgradeWindowView : BindableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<UpgradeWindowView, UxmlTraits>
        {
        }

        private readonly List<TemplateContainer> _rowContainers = new();
        private readonly List<UpgradeStatRowView> _rowViews = new();

        private Button _applyButton;
        private Button _closeButton;
        private Label _availablePointsLabel;
        private VisualElement _backdrop;
        private VisualElement _modalPanel;
        private VisualElement _rowsContainer;

        public IObservable<Unit> OnApplyClicked => _applyButton.OnClickAsObservable();
        public IObservable<Unit> OnCloseClicked => _closeButton.OnClickAsObservable();
        public IObservable<Unit> OnBackdropClicked => _backdrop
            .OnClickEventAsObservable()
            .Where(evt => ReferenceEquals(evt.target, _backdrop))
            .Select(_ => Unit.Default);

        public void SetAvailablePointsText(string text)
        {
            _availablePointsLabel.text = text;
        }

        public void SetApplyEnabled(bool enabled)
        {
            _applyButton.SetEnabled(enabled);
        }

        public IReadOnlyList<UpgradeStatRowView> EnsureRows(int count, VisualTreeAsset rowTemplate)
        {
            if (rowTemplate == null)
            {
                throw new InvalidOperationException("Upgrade row template is not loaded.");
            }

            while (_rowViews.Count < count)
            {
                TemplateContainer container = rowTemplate.CloneTree();
                UpgradeStatRowView rowView = container.Q<UpgradeStatRowView>();
                if (rowView == null)
                {
                    throw new InvalidOperationException("UpgradeStatRowView was not found in row template.");
                }

                _rowContainers.Add(container);
                _rowViews.Add(rowView);
                _rowsContainer.Add(container);
            }

            while (_rowViews.Count > count)
            {
                int lastIndex = _rowViews.Count - 1;
                _rowContainers[lastIndex].RemoveFromHierarchy();
                _rowContainers.RemoveAt(lastIndex);
                _rowViews.RemoveAt(lastIndex);
            }

            return _rowViews;
        }

        protected override void InitializeElements()
        {
            _backdrop = this.Q<VisualElement>("backdrop");
            _modalPanel = this.Q<VisualElement>("modalPanel");
            _availablePointsLabel = this.Q<Label>("availablePointsLabel");
            _rowsContainer = this.Q<VisualElement>("rowsContainer");
            _applyButton = this.Q<Button>("applyButton");
            _closeButton = this.Q<Button>("closeButton");

            _modalPanel.RegisterCallback<ClickEvent>(HandleModalClicked);
        }

        private static void HandleModalClicked(ClickEvent evt) => evt.StopPropagation();
    }
}
