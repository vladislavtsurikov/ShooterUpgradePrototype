using System.Globalization;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.UI.UISystem.Views
{
    public sealed class PlayerHealthHUDView : BindableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<PlayerHealthHUDView, UxmlTraits>
        {
        }

        private Label _healthValueLabel;
        private VisualElement _healthFill;

        public void SetHealthText(float currentValue, float maxValue)
        {
            float safeMaxValue = Mathf.Max(1f, maxValue);
            float normalizedValue = Mathf.Clamp01(currentValue / safeMaxValue);

            _healthFill.style.width = Length.Percent(normalizedValue * 100f);
            _healthValueLabel.text =
                $"{FormatValue(currentValue)}/{FormatValue(maxValue)}";
        }

        protected override void InitializeElements()
        {
            _healthFill = this.Q<VisualElement>("healthFill");
            _healthValueLabel = this.Q<Label>("healthValueLabel");
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
