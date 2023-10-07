using System;
using System.Collections.Generic;
using System.Globalization;
using Extensions.Reflection.Converters;

namespace Extensions.Reflection
{
    public class TypeConverterSettings
    {
        public bool IgnoreCase { get; set; } = true;
        public IFormatProvider IFormatProvider { get; set; } = CultureInfo.InvariantCulture;
        public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.None;
        public TimeSpanStyles TimeSpanStyles { get; set; } = TimeSpanStyles.None;
        public NumberStyles NumberStyles { get; set; } = NumberStyles.Any;
        public string DateTimeFormat { get; set; } = "dd/MM/yyyy";
        public string TimeSpanFormat { get; set; } = "c";
        //public bool AcceptLossyConversion { get; set; } = false;
    }

    public static class TypeConverter
    {
        public static TypeConverterSettings TypeConverterSettings { get; set; } = new TypeConverterSettings();
        public static Dictionary<Type, TypeConverterBase> TypeConverters { get; }
        public static Dictionary<Type, TypeConverterBase> TypeConvertersCache { get; }

        static TypeConverter()
        {
            TypeConverters = new Dictionary<Type, TypeConverterBase>
            {
                [typeof(bool)] = new BoolConverter(),
                [typeof(sbyte)] = new SByteConverter(),
                [typeof(byte)] = new ByteConverter(),
                [typeof(short)] = new ShortConverter(),
                [typeof(ushort)] = new UShortConverter(),
                [typeof(int)] = new IntConverter(),
                [typeof(uint)] = new UIntConverter(),
                [typeof(long)] = new LongConverter(),
                [typeof(ulong)] = new ULongonverter(),
                [typeof(float)] = new FloatConverter(),
                [typeof(double)] = new DoubleConverter(),
                [typeof(decimal)] = new DecimalConverter(),
                [typeof(char)] = new CharConverter(),
                [typeof(DateTime)] = new DateTimeConverter(),
                [typeof(DateTimeOffset)] = new DateTimeOffsetConverter(),
                [typeof(TimeSpan)] = new TimeSpanConverter(),
                [typeof(Guid)] = new GuidConverter(),
                [typeof(bool?)] = new NullableConverter<bool>(),
                [typeof(sbyte?)] = new NullableConverter<sbyte>(),
                [typeof(byte?)] = new NullableConverter<byte>(),
                [typeof(short?)] = new NullableConverter<short>(),
                [typeof(ushort?)] = new NullableConverter<ushort>(),
                [typeof(int?)] = new NullableConverter<int>(),
                [typeof(uint?)] = new NullableConverter<uint>(),
                [typeof(long?)] = new NullableConverter<long>(),
                [typeof(ulong?)] = new NullableConverter<ulong>(),
                [typeof(float?)] = new NullableConverter<float>(),
                [typeof(double?)] = new NullableConverter<double>(),
                [typeof(decimal?)] = new NullableConverter<decimal>(),
                [typeof(char?)] = new NullableConverter<char>(),
                [typeof(DateTime?)] = new NullableConverter<DateTime>(),
                [typeof(DateTimeOffset?)] = new NullableConverter<DateTimeOffset>(),
                [typeof(TimeSpan?)] = new NullableConverter<TimeSpan>(),
                [typeof(Guid?)] = new NullableConverter<Guid>(),
                [typeof(string)] = new StringConverter(),
                [typeof(Uri)] = new UriConverter(),
            };
        }

        public static bool ConvertTo<T>(string value, out T result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverterSettings;

            var type = typeof(T);

            var converter = TypeConverters[type] as TypeConverterBase<T>;

            return converter.Convert(value, out result, typeConverterSettings);
        }

        public static bool ConvertTo(string value, Type targetType, out object result, TypeConverterSettings typeConverterSettings = null)
        {
            typeConverterSettings = typeConverterSettings ?? TypeConverterSettings;

            var converter = TypeConverters[targetType];

            var param = new object[] { value, null, typeConverterSettings };

            var res = (bool)converter.ConvertMethodReflectionCache.Invoke(converter, param);

            result = param[1];
            return res;
        }

        public static bool TryAddConverter<T>(TypeConverterBase<T> typeConverter)
        {
            var type = typeof(T);
            if (TypeConverters.ContainsKey(type))
            {
                return false;
            }

            TypeConverters.Add(type, typeConverter);
            return true;
        }
    }
}
