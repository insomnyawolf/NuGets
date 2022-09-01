#define INPUTDEBUG
using System;
using System.Runtime.InteropServices;
using LowLevelInputHooks.Common;
using LowLevelInputHooks.OsSpecific.Windows;
using static LowLevelInputHooks.Common.InputHookBase;

namespace LowLevelInputHooks
{
    public class InputHook : IDisposable
    {
        private readonly InputHookBase InputHookInstance;

        public bool IsGlobal => InputHookInstance.IsGlobal;
        public event InputEventDelegate OnKeyEvent
        {
            add
            {
                InputHookInstance.OnKeyEvent += value;
            }

            remove
            {
                InputHookInstance.OnKeyEvent -= value;
            }
        }

        public InputHook(bool IsGlobal)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                InputHookInstance = new WindowsInputHook(IsGlobal);
            }
            else
            {
                throw new NotImplementedException($"'{nameof(InputHook)}' is not implemented for '{RuntimeInformation.OSDescription}' yet");
            }

#if INPUTDEBUG
            OnKeyEvent += (InputEvent @event) =>
            {
                Console.WriteLine(@event);
            };
#endif
        }

        // Destructor
        bool IsFinalized = false;

        ~InputHook()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            if (!IsFinalized)
            {
                IsFinalized = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
