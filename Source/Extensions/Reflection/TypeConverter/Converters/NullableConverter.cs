using System;

namespace Extensions.Reflection.Converters
{
    public class NullableConverter<T> : TypeConverterBase<T?> where T : struct
    {
        public override bool Convert(string input, out T? value, TypeConverterSettings typeConverterSettings)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            if (input.Equals("null", typeConverterSettings.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            {
                value = null;
                return true;
            }

            var typeOfT = typeof(T);

            if (!TypeConverter.TypeConverters.TryGetValue(typeOfT, out var typeConverter))
            {
                // tried to convert something that doens't have a converter registered
                throw new Exception("No type converter registered for " + typeOfT.Name);
            }

            var converter = typeConverter as TypeConverterBase<T>;

            var convertStatus = converter.Convert(input, out var valueTemp, typeConverterSettings);

            if (!convertStatus)
            {
                value = null;
                return false;
            }

            value = valueTemp;
            return true;
        }
    }
}
