using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TypeConverterHelper.Converters
{
    public class DateTimeConverter : TypeConverterBase<DateTime>
    {
        public override bool Convert(string value, out DateTime result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return DateTime.TryParseExact(value, typeConverterSettings.DateTimeFormat, typeConverterSettings.IFormatProvider, typeConverterSettings.DateTimeStyles, out result);
        }
    }

    public class DateTimeOffsetConverter : TypeConverterBase<DateTimeOffset>
    {
        public override bool Convert(string value, out DateTimeOffset result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return DateTimeOffset.TryParseExact(value, typeConverterSettings.DateTimeFormat, typeConverterSettings.IFormatProvider, typeConverterSettings.DateTimeStyles, out result);
        }
    }

    public class TimeSpanConverter : TypeConverterBase<TimeSpan>
    {
        private static Regex TimeSpanRegex { get; set; }

        static TimeSpanConverter()
        {
#if NETSTANDARD1_3
            TimeSpanRegex = new Regex(@"^((?<days>\d+)d\s*)?((?<hours>\d+)h\s*)?((?<minutes>\d+)m\s*)?((?<seconds>\d+)s\s*)?$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.RightToLeft | RegexOptions.IgnoreCase);
#else
            TimeSpanRegex = new Regex(@"^((?<days>\d+)d\s*)?((?<hours>\d+)h\s*)?((?<minutes>\d+)m\s*)?((?<seconds>\d+)s\s*)?$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant | RegexOptions.RightToLeft | RegexOptions.IgnoreCase | RegexOptions.Compiled);
#endif
        }

        public override bool Convert(string value, out TimeSpan result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            if (value == "0")
            {
                result = TimeSpan.Zero;
                return true;
            }

            // Wtf?
            if (int.TryParse(value, NumberStyles.Number, typeConverterSettings.IFormatProvider, out _))
            {
                result = TimeSpan.Zero;
                return false;
            }

            if (TimeSpan.TryParseExact(value, typeConverterSettings.TimeSpanFormat, typeConverterSettings.IFormatProvider, typeConverterSettings.TimeSpanStyles, out result))
            {
                return true;
            }

            var m = TimeSpanRegex.Match(value);

            int ds = m.Groups["days"].Success ? int.Parse(m.Groups["days"].Value) : 0;
            int hs = m.Groups["hours"].Success ? int.Parse(m.Groups["hours"].Value) : 0;
            int ms = m.Groups["minutes"].Success ? int.Parse(m.Groups["minutes"].Value) : 0;
            int ss = m.Groups["seconds"].Success ? int.Parse(m.Groups["seconds"].Value) : 0;

            result = new TimeSpan(ds, hs, ms, ss);

            return true;
        }
    }
}
