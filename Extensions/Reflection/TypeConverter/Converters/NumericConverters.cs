using System.Globalization;

namespace Extensions.Reflection.Converters
{
    public class BoolConverter : TypeConverterBase<bool>
    {
        public override bool Convert(string value, out bool result, TypeConverterSettings typeConverterSettings = null)
        {
            return bool.TryParse(value, out result);
        }
    }

    public class CharConverter : TypeConverterBase<char>
    {
        public override bool Convert(string value, out char result, TypeConverterSettings typeConverterSettings = null)
        {
            return char.TryParse(value, out result);
        }
    }

    public class SByteConverter : TypeConverterBase<sbyte>
    {
        public override bool Convert(string value, out sbyte result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return sbyte.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class ByteConverter : TypeConverterBase<byte>
    {
        public override bool Convert(string value, out byte result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return byte.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class ShortConverter : TypeConverterBase<short>
    {
        public override bool Convert(string value, out short result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return short.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class UShortConverter : TypeConverterBase<ushort>
    {
        public override bool Convert(string value, out ushort result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return ushort.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class IntConverter : TypeConverterBase<int>
    {
        public override bool Convert(string value, out int result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return int.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class UIntConverter : TypeConverterBase<uint>
    {
        public override bool Convert(string value, out uint result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return uint.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class LongConverter : TypeConverterBase<long>
    {
        public override bool Convert(string value, out long result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return long.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class ULongonverter : TypeConverterBase<ulong>
    {
        public override bool Convert(string value, out ulong result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return ulong.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class FloatConverter : TypeConverterBase<float>
    {
        public override bool Convert(string value, out float result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return float.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class DoubleConverter : TypeConverterBase<double>
    {
        public override bool Convert(string value, out double result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return double.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }

    public class DecimalConverter : TypeConverterBase<decimal>
    {
        public override bool Convert(string value, out decimal result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverter.TypeConverterSettings;

            return decimal.TryParse(value, typeConverterSettings.NumberStyles, typeConverterSettings.IFormatProvider, out result);
        }
    }
}
