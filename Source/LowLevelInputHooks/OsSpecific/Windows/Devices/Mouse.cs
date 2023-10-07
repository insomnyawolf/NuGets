using System;
using System.Runtime.InteropServices;
using LowLevelInputHooks.Common;

namespace LowLevelInputHooks.OsSpecific.Windows.Devices
{
    internal class WindowsMouseEvent : MouseEvent
    {
        internal WindowsMouseEvent(MouseMessage MouseMessage, MsllHookStruct extra, bool Global)
        {
            Position = extra.Position;

            switch (MouseMessage)
            {
                case MouseMessage.WM_MOUSEMOVE:
                case MouseMessage.WM_NCMOUSEMOVE:
                    InputAction = MouseAction.Move;
                    MouseButton = MouseButton.None;
                    break;
                case MouseMessage.WM_LBUTTONDOWN:
                case MouseMessage.WM_NCLBUTTONDOWN:
                case MouseMessage.WM_LBUTTONDBLCLK:
                case MouseMessage.WM_NCLBUTTONDBLCLK:
                    InputAction = MouseAction.Down;
                    MouseButton = MouseButton.Left;
                    break;
                case MouseMessage.WM_LBUTTONUP:
                case MouseMessage.WM_NCLBUTTONUP:
                    InputAction = MouseAction.Up;
                    MouseButton = MouseButton.Left;
                    break;
                case MouseMessage.WM_RBUTTONDOWN:
                case MouseMessage.WM_NCRBUTTONDOWN:
                case MouseMessage.WM_RBUTTONDBLCLK:
                case MouseMessage.WM_NCRBUTTONDBLCLK:
                    InputAction = MouseAction.Down;
                    MouseButton = MouseButton.Right;
                    break;
                case MouseMessage.WM_RBUTTONUP:
                case MouseMessage.WM_NCRBUTTONUP:
                    InputAction = MouseAction.Up;
                    MouseButton = MouseButton.Right;
                    break;
                case MouseMessage.WM_MBUTTONDOWN:
                case MouseMessage.WM_NCMBUTTONDOWN:
                case MouseMessage.WM_MBUTTONDBLCLK:
                case MouseMessage.WM_NCMBUTTONDBLCLK:
                    InputAction = MouseAction.Down;
                    MouseButton = MouseButton.Middle;
                    break;
                case MouseMessage.WM_MBUTTONUP:
                case MouseMessage.WM_NCMBUTTONUP:
                    InputAction = MouseAction.Up;
                    MouseButton = MouseButton.Middle;
                    break;
                case MouseMessage.WM_MOUSEWHEEL:
                    MouseButton = MouseButton.Scroll;
                    if (Global)
                    {
                        Console.WriteLine(extra.MouseData);
                        if (extra.MouseData > 0)
                            InputAction = MouseAction.Up;
                        else if (extra.MouseData < 0)
                            InputAction = MouseAction.Down;
                    }
                    else
                    {
                        if (extra.WheelDelta > 0)
                            InputAction = MouseAction.Up;
                        else if (extra.WheelDelta < 0)
                            InputAction = MouseAction.Down;
                    }
                    break;
                case MouseMessage.WM_MOUSEHWHEEL:
                    InputAction = MouseAction.None;
                    MouseButton = MouseButton.WheelB;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    internal enum MouseMessage
    {
        WM_MOUSEMOVE = 0x0200,
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_LBUTTONDBLCLK = 0x0203,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205,
        WM_RBUTTONDBLCLK = 0x0206,
        WM_MBUTTONDOWN = 0x0207,
        WM_MBUTTONUP = 0x0208,
        WM_MBUTTONDBLCLK = 0x0209,

        WM_MOUSEWHEEL = 0x020A,
        WM_MOUSEHWHEEL = 0x020E,

        WM_NCMOUSEMOVE = 0x00A0,
        WM_NCLBUTTONDOWN = 0x00A1,
        WM_NCLBUTTONUP = 0x00A2,
        WM_NCLBUTTONDBLCLK = 0x00A3,
        WM_NCRBUTTONDOWN = 0x00A4,
        WM_NCRBUTTONUP = 0x00A5,
        WM_NCRBUTTONDBLCLK = 0x00A6,
        WM_NCMBUTTONDOWN = 0x00A7,
        WM_NCMBUTTONUP = 0x00A8,
        WM_NCMBUTTONDBLCLK = 0x00A9
    }

    /// <summary>
    ///  Specifies the appearance of a button.
    /// </summary>
    [Flags]
    internal enum ButtonState
    {
        Checked = (int)DFCS.CHECKED,
        Flat = (int)DFCS.FLAT,
        Inactive = (int)DFCS.INACTIVE,
        Normal = 0,
        Pushed = (int)DFCS.PUSHED,
        All = Flat | Checked | Pushed | Inactive,
    }

    [Flags]
    internal enum DFCS : uint
    {
        CAPTIONCLOSE = 0x0000,
        CAPTIONMIN = 0x0001,
        CAPTIONMAX = 0x0002,
        CAPTIONRESTORE = 0x0003,
        CAPTIONHELP = 0x0004,
        MENUARROW = 0x0000,
        MENUCHECK = 0x0001,
        MENUBULLET = 0x0002,
        MENUARROWRIGHT = 0x0004,
        SCROLLUP = 0x0000,
        SCROLLDOWN = 0x0001,
        SCROLLLEFT = 0x0002,
        SCROLLRIGHT = 0x0003,
        SCROLLCOMBOBOX = 0x0005,
        SCROLLSIZEGRIP = 0x0008,
        SCROLLSIZEGRIPRIGHT = 0x0010,
        BUTTONCHECK = 0x0000,
        BUTTONRADIOIMAGE = 0x0001,
        BUTTONRADIOMASK = 0x0002,
        BUTTONRADIO = 0x0004,
        BUTTON3STATE = 0x0008,
        BUTTONPUSH = 0x0010,
        INACTIVE = 0x0100,
        PUSHED = 0x0200,
        CHECKED = 0x0400,
        TRANSPARENT = 0x0800,
        HOT = 0x1000,
        ADJUSTRECT = 0x2000,
        FLAT = 0x4000,
        MONO = 0x8000
    }

    [StructLayout(LayoutKind.Sequential)]
    internal class MsllHookStruct
    {
        public Point Position;
        public int MouseData;
        public uint Flags;
        public uint Time;
        public IntPtr dwExtraInfo;
        public int WheelDelta;
    }
}
