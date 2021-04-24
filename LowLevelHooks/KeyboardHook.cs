using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace LowLevelHooks
{
    /// <summary>
    /// Class for intercepting low level keyboard hooks
    /// </summary>
    public class KeyboardHook
    {
        private IntPtr _hookID = IntPtr.Zero;
        private LowLevelKeyboardProc _proc;

        public KeyboardHook()
        {
            _proc = HookCallback;
            _hookID = SetHook(_proc);
        }

        public void UnHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process curProcess = Process.GetCurrentProcess();
            using ProcessModule curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate void KeyEvent(int keyCode);

        public event KeyEvent OnKeyUp;
        public event KeyEvent OnKeyDown;

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode > -1)
            {
                var action = wParam.ToInt32();

                int vkCode = Marshal.ReadInt32(lParam);

                if(action == WM_KEYDOWN || action == WM_SYSKEYDOWN)
                {
                    OnKeyDown?.Invoke(vkCode);
                }
                if (action == WM_KEYUP || action == WM_SYSKEYUP)
                {
                    OnKeyUp?.Invoke(vkCode);
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        /// <summary>
        /// Low-Level function declarations
        /// </summary>

        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x100;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYUP = 0x105;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //private static extern bool GetCursorPos(ref Point pt);
    }
}
