using UnityEngine.InputSystem;

namespace AutoStrike.Input.Services.States
{
    public sealed class GamepadInputModeState : InputModeState
    {
        public GamepadInputModeState(MobileInputVirtualGamepad mobileInputVirtualGamepad)
            : base(mobileInputVirtualGamepad)
        {
        }

        public override string Id => InputModeIds.Gamepad;

        protected override bool MatchesDevice(InputDevice device) =>
            device is Gamepad && !MobileInputVirtualGamepad.Owns(device);
    }
}
