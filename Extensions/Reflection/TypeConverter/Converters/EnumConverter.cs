using System;

namespace Extensions.Reflection.Converters
{
    public class EnumConverter<T> : TypeConverterBase<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        public override bool Convert(string value, out T result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return Enum.TryParse(value, typeConverterSettings.IgnoreCase, out result);
        }
    }
}
