using System.Text.Json;
using System.Text.Json.Serialization;
// Using this for now till i define real common ones
using LowLevelInputHooks.OsSpecific.Windows.Devices;

namespace LowLevelInputHooks.Common
{
    public class InputEvent : EventArgs
    {
        public InputOrigin InputOrigin { get; set; }
        public KeyEvent? KeyEvent { get; set; }
        public MouseEvent? MouseEvent { get; set; }

        internal InputEvent() { }

        private static readonly JsonSerializerOptions JsonSerializerOptions;
        static InputEvent()
        {
            JsonSerializerOptions = new()
            {
                AllowTrailingCommas = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                IncludeFields = true,
            };

            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, options: JsonSerializerOptions);
        }
    }

    public class KeyEvent : InputEventBase
    {
        public Keys Key { get; protected set; }
        public KeyAction InputAction { get; protected set; }
        public bool Shift { get; protected set; }
        public bool Ctrl { get; protected set; }
        public bool Alt { get; protected set; }

        internal KeyEvent(Keys Key, KeyAction InputType, bool Shift, bool Ctrl, bool Alt)
        {
            this.Key = Key;
            this.Shift = Shift;
            this.Ctrl = Ctrl;
            this.Alt = Alt;
            InputAction = InputType;
        }
    }

    public class MouseEvent : InputEventBase
    {
        public MouseButton MouseButton { get; protected set; }
        public MouseAction InputAction { get; protected set; }
        public Point Position { get; protected set; }
    }

    // Just exist to ease debugging propuses
    public abstract class InputEventBase
    {
        private static readonly JsonSerializerOptions JsonSerializerOptions;
        static InputEventBase()
        {
            JsonSerializerOptions = new()
            {
                AllowTrailingCommas = true,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.Never,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReadCommentHandling = JsonCommentHandling.Skip,
                WriteIndented = true,
                IncludeFields = true,
            };

            JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this, options: JsonSerializerOptions);
        }
    }
}
