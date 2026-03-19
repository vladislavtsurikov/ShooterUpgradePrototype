using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace AutoStrike.MobileInputUI.MobileInputUI.Runtime
{
    public sealed class MobileMoveStickView : BindableVisualElement
    {
        public readonly struct StickPointerData
        {
            public StickPointerData(Vector2 localPosition, Vector2 padCenter, float radius)
            {
                LocalPosition = localPosition;
                PadCenter = padCenter;
                Radius = radius;
            }

            public Vector2 LocalPosition { get; }
            public Vector2 PadCenter { get; }
            public float Radius { get; }
        }

        public new class UxmlFactory : UxmlFactory<MobileMoveStickView, UxmlTraits>
        {
        }

        private readonly Subject<StickPointerData> _pointerChanged = new();
        private readonly Subject<Unit> _released = new();

        private VisualElement _pad;
        private VisualElement _knob;
        private int _activePointerId = -1;

        public IObservable<StickPointerData> OnPointerChanged => _pointerChanged;
        public IObservable<Unit> OnReleased => _released;

        protected override void InitializeElements()
        {
            _pad = this.Q<VisualElement>("mobile-stick-pad");
            _knob = this.Q<VisualElement>("mobile-stick-knob-shadow");

            RegisterCallback<PointerDownEvent>(HandlePointerDown);
            RegisterCallback<PointerMoveEvent>(HandlePointerMove);
            RegisterCallback<PointerUpEvent>(HandlePointerUp);
            RegisterCallback<PointerCancelEvent>(HandlePointerCancel);
            RegisterCallback<PointerCaptureOutEvent>(HandlePointerCaptureOut);
            RegisterCallback<DetachFromPanelEvent>(_ => ReleaseStick());
        }

        private void HandlePointerDown(PointerDownEvent evt)
        {
            if (_activePointerId != -1 || _pad == null || !_pad.worldBound.Contains(evt.position))
            {
                return;
            }

            _activePointerId = evt.pointerId;
            PointerCaptureHelper.CapturePointer(this, _activePointerId);
            NotifyPointerChanged(evt.position);
            evt.StopPropagation();
        }

        private void HandlePointerMove(PointerMoveEvent evt)
        {
            if (evt.pointerId != _activePointerId)
            {
                return;
            }

            NotifyPointerChanged(evt.position);
            evt.StopPropagation();
        }

        private void HandlePointerUp(PointerUpEvent evt)
        {
            if (evt.pointerId != _activePointerId)
            {
                return;
            }

            ReleaseStick();
            evt.StopPropagation();
        }

        private void HandlePointerCancel(PointerCancelEvent evt)
        {
            if (evt.pointerId != _activePointerId)
            {
                return;
            }

            ReleaseStick();
            evt.StopPropagation();
        }

        private void HandlePointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (_activePointerId != -1)
            {
                ReleaseStick();
            }
        }

        public void SetKnobOffset(Vector2 knobOffset)
        {
            if (_knob == null)
            {
                return;
            }

            _knob.transform.position = new Vector3(knobOffset.x, knobOffset.y, 0f);
        }

        public void ResetKnob()
        {
            if (_knob != null)
            {
                _knob.transform.position = Vector3.zero;
            }
        }

        private void NotifyPointerChanged(Vector2 worldPosition)
        {
            if (!TryCreatePointerData(worldPosition, out StickPointerData pointerData))
            {
                return;
            }

            _pointerChanged.OnNext(pointerData);
        }

        private bool TryCreatePointerData(Vector2 worldPosition, out StickPointerData pointerData)
        {
            pointerData = default;

            if (_pad == null || _knob == null)
            {
                return false;
            }

            Vector2 localPosition = _pad.WorldToLocal(worldPosition);
            Rect padRect = _pad.contentRect;
            Vector2 padCenter = padRect.center;
            float radius = CalculateRadius(padRect);

            pointerData = new StickPointerData(localPosition, padCenter, radius);
            return true;
        }

        private float CalculateRadius(Rect padRect)
        {
            float knobWidth = Mathf.Max(_knob.layout.width, _knob.resolvedStyle.width);
            float knobHalfSize = knobWidth * 0.5f;
            float padRadius = Mathf.Min(padRect.width, padRect.height) * 0.5f;

            return Mathf.Max(1f, padRadius - knobHalfSize - 8f);
        }

        private void ReleaseStick()
        {
            if (_activePointerId != -1 && PointerCaptureHelper.HasPointerCapture(this, _activePointerId))
            {
                PointerCaptureHelper.ReleasePointer(this, _activePointerId);
            }

            _activePointerId = -1;
            ResetKnob();
            _released.OnNext(Unit.Default);
        }
    }
}
