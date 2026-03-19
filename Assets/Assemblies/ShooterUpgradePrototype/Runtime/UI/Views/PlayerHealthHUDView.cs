using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    public sealed class PlayerHealthHUDView : BindableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<PlayerHealthHUDView, UxmlTraits>
        {
        }

        private float _currentValue;
        private Label _healthValueLabel;
        private VisualElement _healthFill;
        private bool _isInitialized;
        private float _maxValue;

        public void SetHealthText(float currentValue, float maxValue)
        {
            _currentValue = currentValue;
            _maxValue = maxValue;

            if (_isInitialized)
            {
                ApplyHealthState();
            }
        }

        protected override void InitializeElements()
        {
            _healthFill = this.Q<VisualElement>("healthFill");
            _healthValueLabel = this.Q<Label>("healthValueLabel");
            _isInitialized = true;

            ApplyHealthState();
        }

        private void ApplyHealthState()
        {
            float safeMaxValue = Mathf.Max(1f, _maxValue);
            float normalizedValue = Mathf.Clamp01(_currentValue / safeMaxValue);

            _healthFill.style.width = Length.Percent(normalizedValue * 100f);
            _healthValueLabel.text = $"{FormatValue(_currentValue)}/{FormatValue(_maxValue)}";
        }

        private static string FormatValue(float value)
        {
            if (Mathf.Abs(value - Mathf.Round(value)) < 0.01f)
            {
                return Mathf.RoundToInt(value).ToString(CultureInfo.InvariantCulture);
            }

            return value.ToString("0.#", CultureInfo.InvariantCulture);
        }
    }
}
