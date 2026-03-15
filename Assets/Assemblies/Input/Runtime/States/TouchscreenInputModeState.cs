using UnityEngine.InputSystem;

namespace AutoStrike.Input.Services.States
{
    public sealed class TouchscreenInputModeState : InputModeState
    {
        public TouchscreenInputModeState(MobileInputVirtualGamepad mobileInputVirtualGamepad)
            : base(mobileInputVirtualGamepad)
        {
        }

        public override string Id => InputModeIds.Touch;

        protected override bool MatchesDevice(InputDevice device) =>
            device is Touchscreen || MobileInputVirtualGamepad.Owns(device);
    }
}
