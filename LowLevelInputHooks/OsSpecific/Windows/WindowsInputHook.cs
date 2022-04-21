using System.Runtime.InteropServices;
using LowLevelInputHooks.Common;
using LowLevelInputHooks.OsSpecific.Windows.Devices;

namespace LowLevelInputHooks.OsSpecific.Windows
{
    // Based on https://stackoverflow.com/questions/46013287/c-sharp-global-keyboard-hook-that-opens-a-form-from-a-console-application
    internal class WindowsInputHook : InputHookBase, IDisposable
    {
        private delegate int CallbackDelegate(int Code, IntPtr W, IntPtr L);

        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(HookType idHook, CallbackDelegate lpfn, int hInstance, int threadId);
        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);
        [DllImport("user32", CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int GetCurrentThreadId();
        [DllImport("user32.dll")]
        private static extern short GetKeyState(Keys nVirtKey);

        private readonly int KeyboardHookID;
        private readonly CallbackDelegate OnKeyboardHookCallBack;

        private readonly int MouseHookID;
        private readonly CallbackDelegate OnMouseHookCallBack;

        private enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        //Start hook
        internal WindowsInputHook(bool IsGlobal) : base(IsGlobal)
        {
            OnKeyboardHookCallBack = new CallbackDelegate(KeybHookProc);
            OnMouseHookCallBack = new CallbackDelegate(MouseHookProc);

            //0 for local hook. eller hwnd til user32 for global
            int hInstance = 0;

            if (IsGlobal)
            {
                //0 for global hook. eller thread for hooken
                int threadId = 0;

                // Keyboard
                KeyboardHookID = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, OnKeyboardHookCallBack, hInstance, threadId);
                //Mouse
                MouseHookID = SetWindowsHookEx(HookType.WH_MOUSE_LL, OnMouseHookCallBack, hInstance, threadId);
            }
            else
            {
                //0 for global hook. eller thread for hooken
                int threadId = GetCurrentThreadId();

                //Keyboard
                KeyboardHookID = SetWindowsHookEx(HookType.WH_KEYBOARD, OnKeyboardHookCallBack, hInstance, threadId);
                //Mouse
                MouseHookID = SetWindowsHookEx(HookType.WH_MOUSE, OnMouseHookCallBack, hInstance, threadId);
            }
        }

        #region keyboard

        private int KeybHookProc(int Code, IntPtr W, IntPtr L)
        {
            if (Code < 0)
            {
                return CallNextHookEx(KeyboardHookID, Code, W, L);
            }
            try
            {
                var input = new InputEvent()
                {
                    InputOrigin = InputOrigin.Keyboard,
                };

                if (!IsGlobal)
                {
                    // Overflow if you don't use int64
                    var keyState = L.ToInt64() >> 30;

                    if (keyState == 0)
                    {
                        input.KeyEvent = new KeyEvent((Keys)W, KeyAction.Down, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else if (keyState == 3)
                    {
                        input.KeyEvent = new KeyEvent((Keys)W, KeyAction.Up, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else if (keyState == 1)
                    {
                        input.KeyEvent = new KeyEvent((Keys)W, KeyAction.Repeat, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else if (keyState == 2)
                    {
#warning investigate what keyState 2 really is
                        // Windows keyboard shortcut?
                        goto exit;
                    }
                    else
                    {
                        throw new NotImplementedException($"keyState => {keyState}");
                    }
                }
                else
                {
                    RawKeyEvents kEvent = (RawKeyEvents)W;

                    // Virtual Key Code
                    var vkCode = Marshal.ReadInt32(L);

                    if (kEvent == RawKeyEvents.KeyDown || kEvent == RawKeyEvents.SKeyDown)
                    {
                        input.KeyEvent = new KeyEvent((Keys)vkCode, KeyAction.Down, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else if (kEvent == RawKeyEvents.KeyUp || kEvent == RawKeyEvents.SKeyUp)
                    {
                        input.KeyEvent = new KeyEvent((Keys)vkCode, KeyAction.Up, GetShiftPressed(), GetCtrlPressed(), GetAltPressed());
                    }
                    else
                    {
                        throw new NotImplementedException($"keyState => {kEvent}");
                    }
                }

                Invoke(input);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine(ex.ToString());
                throw;
#else
#warning shall we really?
                //Ignore all errors...
#endif
            }

        exit:
            return CallNextHookEx(KeyboardHookID, Code, W, L);
        }

        //public static bool GetCapslock()
        //{
        //    return Convert.ToBoolean(GetKeyState(Keys.CapsLock)) & true;
        //}
        //public static bool GetNumlock()
        //{
        //    return Convert.ToBoolean(GetKeyState(Keys.NumLock)) & true;
        //}
        //public static bool GetScrollLock()
        //{
        //    return Convert.ToBoolean(GetKeyState(Keys.Scroll)) & true;
        //}

#warning optimize this via getting keystate as maybe it's not needed? (will make a wrapper that returns an inputstate where you can ask the current key state.

        public static bool GetShiftPressed()
        {
            int state = GetKeyState(Keys.ShiftKey);
            if (state < -1 || state > 1)
                return true;
            return false;
        }
        public static bool GetCtrlPressed()
        {
            int state = GetKeyState(Keys.ControlKey);
            if (state < -1 || state > 1)
                return true;
            return false;
        }
        public static bool GetAltPressed()
        {
            int state = GetKeyState(Keys.Menu);
            if (state < -1 || state > 1)
                return true;
            return false;
        }

        #endregion keyboard

        #region mouse
        private int MouseHookProc(int Code, IntPtr W, IntPtr L)
        {
            if (Code < 0)
            {
                return CallNextHookEx(MouseHookID, Code, W, L);
            }

            var mouseExtraData = new MsllHookStruct();
            Marshal.PtrToStructure(L, mouseExtraData);

            var input = new InputEvent()
            {
                InputOrigin = InputOrigin.Mouse,
                MouseEvent = new WindowsMouseEvent((MouseMessage)W, mouseExtraData, IsGlobal)
            };

            Invoke(input);

            // Pass the hook information to the next hook procedure in chain
            return CallNextHookEx(MouseHookID, Code, W, L);
        }

        #endregion mouse

        // Destructor
        bool IsFinalized = false;

        ~WindowsInputHook()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (!IsFinalized)
            {
                base.Dispose();
                UnhookWindowsHookEx(KeyboardHookID);
                UnhookWindowsHookEx(MouseHookID);
                IsFinalized = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}