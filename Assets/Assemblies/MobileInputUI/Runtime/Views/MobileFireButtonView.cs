using System;
using UniRx;
using UnityEngine.UIElements;
using VladislavTsurikov.UISystem.Runtime.UIToolkitIntegration;

namespace AutoStrike.MobileInputUI.MobileInputUI.Runtime
{
    public sealed class MobileFireButtonView : BindableVisualElement
    {
        public new class UxmlFactory : UxmlFactory<MobileFireButtonView, UxmlTraits>
        {
        }

        private readonly Subject<bool> _pressedChanged = new();

        private VisualElement _button;
        private int _activePointerId = -1;

        public IObservable<bool> OnPressedChanged => _pressedChanged;

        protected override void InitializeElements()
        {
            _button = this.Q<VisualElement>("mobile-fire-button");
            if (_button == null)
            {
                return;
            }

            _button.RegisterCallback<PointerDownEvent>(HandlePointerDown);
            _button.RegisterCallback<PointerUpEvent>(HandlePointerUp);
            _button.RegisterCallback<PointerCancelEvent>(HandlePointerCancel);
            _button.RegisterCallback<PointerCaptureOutEvent>(HandlePointerCaptureOut);
            RegisterCallback<DetachFromPanelEvent>(_ => ReleaseFireButton());
        }

        private void HandlePointerDown(PointerDownEvent evt)
        {
            if (_activePointerId != -1)
            {
                return;
            }

            _activePointerId = evt.pointerId;
            PointerCaptureHelper.CapturePointer(_button, _activePointerId);
            _pressedChanged.OnNext(true);
            evt.StopPropagation();
        }

        private void HandlePointerUp(PointerUpEvent evt)
        {
            if (evt.pointerId != _activePointerId)
            {
                return;
            }

            ReleaseFireButton();
            evt.StopPropagation();
        }

        private void HandlePointerCancel(PointerCancelEvent evt)
        {
            if (evt.pointerId != _activePointerId)
            {
                return;
            }

            ReleaseFireButton();
            evt.StopPropagation();
        }

        private void HandlePointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (_activePointerId != -1)
            {
                ReleaseFireButton();
            }
        }

        private void ReleaseFireButton()
        {
            if (_button != null
                && _activePointerId != -1
                && PointerCaptureHelper.HasPointerCapture(_button, _activePointerId))
            {
                PointerCaptureHelper.ReleasePointer(_button, _activePointerId);
            }

            _activePointerId = -1;
            _pressedChanged.OnNext(false);
        }
    }
}
