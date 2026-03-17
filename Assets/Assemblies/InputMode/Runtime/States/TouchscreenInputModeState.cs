namespace AutoStrike.Input.Services.States
{
    public sealed class TouchscreenInputModeState : InputModeState
    {
        public override string Name => "Touch";

        public override int GetPriority(UnityEngine.InputSystem.InputDevice lastUsedDevice)
        {
            if (!HasTouchscreen())
            {
                return -1;
            }

            return IsTouchDevice(lastUsedDevice) ? 150 : 100;
        }

        private static bool HasTouchscreen() =>
            UnityEngine.Application.isMobilePlatform;

        private static bool IsTouchDevice(UnityEngine.InputSystem.InputDevice device) =>
            device is UnityEngine.InputSystem.Touchscreen;
    }
}
