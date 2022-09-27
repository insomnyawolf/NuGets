using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InMemoryDatabase
{
    public class DatabaseEntryConverter : JsonConverterFactory
    {
        private static readonly Dictionary<Guid, JsonConverter> ConverterCache = new Dictionary<Guid, JsonConverter>();

        private static readonly Type TargetType = typeof(DatabaseEntry<>);

        public override bool CanConvert(Type typeToConvert)
        {
            if (!typeToConvert.IsGenericType)
            {
                return false;
            }

            if (typeToConvert.GetGenericTypeDefinition() != TargetType)
            {
                return false;
            }

            return true;
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            if (ConverterCache.TryGetValue(type.GUID, out var converter))
            {
                return converter;
            }

            Type keyType = type.GetGenericArguments()[0];

            var temp = typeof(DatabaseEntryConverterInner<>).MakeGenericType(keyType);

            converter = (JsonConverter)Activator.CreateInstance(
               temp,
               BindingFlags.Instance | BindingFlags.Public,
               binder: null,
               args: new object[] { keyType }, //Array.Empty<object>(),
               culture: null
               );

            ConverterCache.Add(type.GUID, converter);

            return converter;
        }

        protected class DatabaseEntryConverterInner<T> : JsonConverter<DatabaseEntry<T>> where T : class
        {
            public Type ContentType { get; set; }

            protected DatabaseEntryConverterInner(Type ContentType)
            {
                this.ContentType = ContentType;
            }

            public override DatabaseEntry<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new DatabaseEntry<T>()
                {
                    Value = JsonSerializer.Deserialize<T>(ref reader, options)
                };
            }

#warning Extremately slow, need optimization

            [Obsolete("Extremately slow if you use more than 2 million entries, need optimization")]
            public override void Write(Utf8JsonWriter writer, DatabaseEntry<T> value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value.Value, options);
            }
        }
    }
}
