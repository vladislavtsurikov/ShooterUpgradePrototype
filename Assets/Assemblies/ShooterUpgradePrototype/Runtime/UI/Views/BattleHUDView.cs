using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace ShooterUpgradePrototype.ShooterUpgradePrototype.Runtime
{
    public sealed class BattleHUDView : BindableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<BattleHUDView, UxmlTraits>
        {
        }

        private VisualElement _topRow;
        private float _basePaddingTop;
        private float _basePaddingLeft;
        private float _basePaddingRight;
        private bool _basePaddingCaptured;

        protected override void InitializeElements()
        {
            _topRow = this.Q<VisualElement>(className: "hud-top-row");

            RegisterCallback<AttachToPanelEvent>(_ => ApplySafeAreaInsets());
            RegisterCallback<GeometryChangedEvent>(_ => ApplySafeAreaInsets());
        }

        private void ApplySafeAreaInsets()
        {
            if (_topRow == null || panel == null)
            {
                return;
            }

            if (!_basePaddingCaptured)
            {
                _basePaddingTop = _topRow.resolvedStyle.paddingTop;
                _basePaddingLeft = _topRow.resolvedStyle.paddingLeft;
                _basePaddingRight = _topRow.resolvedStyle.paddingRight;
                _basePaddingCaptured = true;
            }

            Rect safe = Screen.safeArea;
            float leftInsetPx = Mathf.Max(0f, safe.xMin);
            float rightInsetPx = Mathf.Max(0f, Screen.width - safe.xMax);
            float topInsetPx = Mathf.Max(0f, Screen.height - safe.yMax);

            Vector2 leftInsetPanel = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(leftInsetPx, 0f));
            Vector2 rightInsetPanel = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(rightInsetPx, 0f));
            Vector2 topInsetPanel = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(0f, topInsetPx));

            _topRow.style.paddingLeft = _basePaddingLeft + leftInsetPanel.x;
            _topRow.style.paddingRight = _basePaddingRight + rightInsetPanel.x;
            _topRow.style.paddingTop = _basePaddingTop + topInsetPanel.y;
        }
    }
}
