using System.Runtime.InteropServices;

namespace LowLevelInputHooks.Common
{
    public enum InputOrigin
    {
        Mouse,
        Keyboard,
    }

    public enum KeyAction
    {
        Down,
        Up,
#warning only works on non global inputs
        Repeat,
    }

    public enum MouseAction
    {
        Down,
        Up,
        Move,
        None
    }

    public enum MouseButton
    {
        None,
        Left,
        Right,
        Middle,
        Scroll,
        WheelB,
    }

    [StructLayout(LayoutKind.Sequential)]
    public class Point
    {
        public int X;
        public int Y;
    }
}
