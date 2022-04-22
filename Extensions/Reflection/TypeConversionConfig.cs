using System.Globalization;
using System.Threading;

namespace Extensions.Reflection
{
    public class TypeConversionConfig
    {
        public bool AcceptLossyConversion { get; set; } = false;
        public string DateTimeFormat { get; set; } = "dd/MM/yyyy";
        public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.AllowWhiteSpaces;
        public string TimeSpanFormat { get; set; } = "c";
        public TimeSpanStyles TimeSpanStyles { get; set; } = TimeSpanStyles.None;
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Any;
        public CultureInfo CultureInfo { get; set; } = Thread.CurrentThread.CurrentCulture;
    }
}
