namespace LowLevelInputHooks.Common
{
    public abstract class InputHookBase : IDisposable
    {
        public delegate void InputEventDelegate(InputEvent @event);
        public event InputEventDelegate OnKeyEvent;
        public bool IsGlobal { get; }

        public InputHookBase(bool IsGlobal)
        {
            this.IsGlobal = IsGlobal;
        }

        public void Invoke(InputEvent value)
        {
            OnKeyEvent?.Invoke(value);
        }

        // Destructor
        bool IsFinalized = false;

        ~InputHookBase()
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
