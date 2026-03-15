namespace AutoStrike.Input.Services.States
{
    public sealed class KeyboardMouseInputModeState : InputModeState
    {
        public override string Name => "KeyboardMouse";

        public override int GetPriority(UnityEngine.InputSystem.InputDevice lastUsedDevice)
        {
            if (!HasKeyboardMouse())
            {
                return -1;
            }

            if (IsKeyboardMouseDevice(lastUsedDevice))
            {
                return 310;
            }

            return 10;
        }

        private static bool HasKeyboardMouse() =>
            UnityEngine.InputSystem.Keyboard.current != null || UnityEngine.InputSystem.Mouse.current != null;

        private static bool IsKeyboardMouseDevice(UnityEngine.InputSystem.InputDevice device) =>
            device is UnityEngine.InputSystem.Keyboard || device is UnityEngine.InputSystem.Mouse;
    }
}
