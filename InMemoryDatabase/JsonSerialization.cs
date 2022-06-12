using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
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
            if (ConverterCache.ContainsKey(type.GUID))
            {
                return ConverterCache[type.GUID];
            }

            Type keyType = type.GetGenericArguments()[0];

            var temp = typeof(DatabaseEntryConverterInner<>).MakeGenericType(keyType);

#pragma warning disable CS8600 // They are null on propouse
            var converter = (JsonConverter)Activator.CreateInstance(
                temp,
                BindingFlags.Instance | BindingFlags.Public,
                binder: null,
                args: Array.Empty<object>(), //new object[] { options },
                culture: null
                );

#pragma warning restore CS8600
            ConverterCache.Add(type.GUID, converter);

            return converter;
        }

        private class DatabaseEntryConverterInner<T> : JsonConverter<DatabaseEntry<T>> where T : class
        {
            // Does this even do anything? ;-;
            private static JsonValue JsonValue;
            
            public DatabaseEntryConverterInner(JsonSerializerOptions JsonSerializerOptions)
            {
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
