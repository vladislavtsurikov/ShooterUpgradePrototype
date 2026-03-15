namespace AutoStrike.Input.Services
{
    public static class InputModeIds
    {
        public const string Auto = "Auto";
        public const string KeyboardMouse = "KeyboardMouse";
        public const string Touch = "Touch";
        public const string Gamepad = "Gamepad";

        public static readonly string[] Preferences =
        {
            Auto,
            KeyboardMouse,
            Touch,
            Gamepad
        };

        public static string NormalizePreference(string preference)
        {
            if (string.IsNullOrWhiteSpace(preference))
            {
                return Auto;
            }

            foreach (string value in Preferences)
            {
                if (value == preference)
                {
                    return value;
                }
            }

            return Auto;
        }
    }
}
