namespace AutoStrike.Input.Services.States
{
    public sealed class GamepadInputModeState : InputModeState
    {
        private const string VirtualGamepadProductName = "AutoStrike.MobileVirtualGamepad";

        public override string Name => "Gamepad";

        public override int GetPriority(UnityEngine.InputSystem.InputDevice lastUsedDevice)
        {
            if (!HasPhysicalGamepad())
            {
                return -1;
            }

            return IsPhysicalGamepadDevice(lastUsedDevice) ? 300 : 200;
        }

        private static bool HasPhysicalGamepad()
        {
            for (int i = 0; i < UnityEngine.InputSystem.Gamepad.all.Count; i++)
            {
                if (IsPhysicalGamepadDevice(UnityEngine.InputSystem.Gamepad.all[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsPhysicalGamepadDevice(UnityEngine.InputSystem.InputDevice device) =>
            device is UnityEngine.InputSystem.Gamepad
            && device.description.product != VirtualGamepadProductName;
    }
}
