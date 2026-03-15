namespace AutoStrike.Input.Services.States
{
    public sealed class GamepadInputModeState : InputModeState
    {
        public override string Name => "Gamepad";

        public override int GetPriority(UnityEngine.InputSystem.InputDevice lastUsedDevice)
        {
            if (!HasPhysicalGamepad())
            {
                return -1;
            }

            return IsPhysicalGamepadDevice(lastUsedDevice) ? 300 : 200;
        }

        private static bool HasPhysicalGamepad() => UnityEngine.InputSystem.Gamepad.all.Count > 0;

        private static bool IsPhysicalGamepadDevice(UnityEngine.InputSystem.InputDevice device) =>
            device is UnityEngine.InputSystem.Gamepad;
    }
}
