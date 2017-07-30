public enum ControllerType
{
    KeyboardMouse,
    xBox360Windows,
    xBox360Mac,
    xBox360Linux
}

public static class Controllers {

    public static class xBox360
    {
        public static class Windows
        {
            public static string ShootButton(string num) { return "joystick " + num + " button 5"; }
            public static string MoveAxisX(string num) { return "Joystick " + num + " X axis"; }
            public static string MoveAxisY(string num) { return "Joystick " + num + " Y axis"; }
            public static string AimAxisX(string num) { return "Joystick " + num + " 4th axis"; }
            public static string AimAxisY(string num) { return "Joystick " + num + " 5th axis"; }
        }

        public static class Mac
        {
            public static string ShootButton(string num) { return "joystick " + num + " button 14"; }
            public static string MoveAxisX(string num) { return "Joystick " + num + " X axis"; }
            public static string MoveAxisY(string num) { return "Joystick " + num + " Y axis"; }
            public static string AimAxisX(string num) { return "Joystick " + num + " 3rd axis"; }
            public static string AimAxisY(string num) { return "Joystick " + num + " 4th axis"; }
        }

        public static class Linux
        {
            public static string ShootButton(string num) { return "joystick " + num + " button 5"; }
            public static string MoveAxisX(string num) { return "Joystick " + num + " X axis"; }
            public static string MoveAxisY(string num) { return "Joystick " + num + " Y axis"; }
            public static string AimAxisX(string num) { return "Joystick " + num + " 4th axis"; }
            public static string AimAxisY(string num) { return "Joystick " + num + " 5th axis"; }
        }
    }
}
