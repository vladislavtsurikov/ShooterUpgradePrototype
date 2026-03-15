using UnityEngine.InputSystem;

namespace AutoStrike.Input.Services.States
{
    public sealed class KeyboardMouseInputModeState : InputModeState
    {
        public KeyboardMouseInputModeState(MobileInputVirtualGamepad mobileInputVirtualGamepad)
            : base(mobileInputVirtualGamepad)
        {
        }

        public override string Id => InputModeIds.KeyboardMouse;

        protected override bool MatchesDevice(InputDevice device)
        {
            if (device == null)
            {
                return false;
            }

            if (MobileInputVirtualGamepad.Owns(device))
            {
                return false;
            }

            return device is not Gamepad && device is not Touchscreen;
        }
    }
}
