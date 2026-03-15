using System;
using UniRx;
using UnityEngine;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace AutoStrike.MobileInputUI.Views
{
    public sealed class MobileMoveStickView : BindableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<MobileMoveStickView, UxmlTraits>
        {
        }

        private readonly Subject<Vector2> _inputChanged = new();
        private readonly Subject<Unit> _released = new();

        private VisualElement _pad;
        private VisualElement _knob;
        private int _activePointerId = -1;

        public IObservable<Vector2> OnInputChanged => _inputChanged;
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
            UpdateStick(evt.position);
            evt.StopPropagation();
        }

        private void HandlePointerMove(PointerMoveEvent evt)
        {
            if (evt.pointerId != _activePointerId)
            {
                return;
            }

            UpdateStick(evt.position);
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

        private void UpdateStick(Vector2 worldPosition)
        {
            if (_pad == null || _knob == null)
            {
                return;
            }

            Vector2 localPosition = _pad.WorldToLocal(worldPosition);
            Rect padRect = _pad.contentRect;
            Vector2 padCenter = padRect.center;
            float radius = CalculateRadius(padRect);
            Vector2 knobOffset = Vector2.ClampMagnitude(localPosition - padCenter, radius);

            _knob.transform.position = new Vector3(knobOffset.x, knobOffset.y, 0f);
            _inputChanged.OnNext(radius <= Mathf.Epsilon ? Vector2.zero : knobOffset / radius);
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

            if (_knob != null)
            {
                _knob.transform.position = Vector3.zero;
            }

            _released.OnNext(Unit.Default);
        }
    }
}
