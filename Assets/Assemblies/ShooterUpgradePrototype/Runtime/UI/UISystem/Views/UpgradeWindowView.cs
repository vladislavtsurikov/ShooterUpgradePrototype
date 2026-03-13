using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class UpgradeWindowView : VisualElement, IBindableUIElement
    {
        private readonly Subject<Unit> _applyClicked = new();
        private readonly Subject<Unit> _closeClicked = new();
        private readonly Subject<Unit> _backdropClicked = new();
        private readonly List<TemplateContainer> _rowContainers = new();
        private readonly List<UpgradeStatRowView> _rowViews = new();

        private Button _applyButton;
        private Button _closeButton;
        private Label _availablePointsLabel;
        private VisualElement _backdrop;
        private VisualElement _modalPanel;
        private VisualElement _rowsContainer;
        private bool _initialized;

        public new class UxmlFactory : UxmlFactory<UpgradeWindowView, UxmlTraits>
        {
        }

        public UpgradeWindowView() => RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);

        public string BindingId => nameof(UpgradeWindowView);
        public VisualElement Element => this;
        public IObservable<Unit> OnApplyClicked => _applyClicked;
        public IObservable<Unit> OnCloseClicked => _closeClicked;
        public IObservable<Unit> OnBackdropClicked => _backdropClicked;

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

        private void HandleAttachToPanel(AttachToPanelEvent evt)
        {
            if (_initialized)
            {
                return;
            }

            _backdrop = this.Q<VisualElement>("backdrop");
            _modalPanel = this.Q<VisualElement>("modalPanel");
            _availablePointsLabel = this.Q<Label>("availablePointsLabel");
            _rowsContainer = this.Q<VisualElement>("rowsContainer");
            _applyButton = this.Q<Button>("applyButton");
            _closeButton = this.Q<Button>("closeButton");

            if (_backdrop == null || _modalPanel == null || _availablePointsLabel == null ||
                _rowsContainer == null || _applyButton == null || _closeButton == null)
            {
                throw new InvalidOperationException("UpgradeWindowView is missing required UI elements.");
            }

            _applyButton.clicked += () => _applyClicked.OnNext(Unit.Default);
            _closeButton.clicked += () => _closeClicked.OnNext(Unit.Default);
            _backdrop.RegisterCallback<ClickEvent>(HandleBackdropClicked);
            _modalPanel.RegisterCallback<ClickEvent>(HandleModalClicked);

            _initialized = true;
        }

        private void HandleBackdropClicked(ClickEvent evt)
        {
            if (ReferenceEquals(evt.target, _backdrop))
            {
                _backdropClicked.OnNext(Unit.Default);
            }
        }

        private static void HandleModalClicked(ClickEvent evt) => evt.StopPropagation();
    }
}
