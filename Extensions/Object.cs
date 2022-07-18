using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Extensions
{
    public static class Object
    {
        public static TClass[] AsArray<TClass>(this TClass item)
        {
            return new TClass[] { item };
        }

        public static T CloneBinaryFormatter<T>(this T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

            // Don't serialize a null object, simply return the default for that object
            if (source is null)
            {
                return default;
            }

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T CloneJsonSerializer<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (source is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(source));
        }

        public static MemoryStream ToStream<TClass>(this TClass source)
        {
            if (!typeof(TClass).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

            // Don't serialize a null object, simply return the default for that object
            if (source is null)
            {
                return default;
            }

            IFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return stream;
            }
        }

        public static TClass ToObject<TClass>(this Stream source)
        {
            if (!typeof(TClass).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", nameof(source));
            }

            IFormatter formatter = new BinaryFormatter();
            return (TClass)formatter.Deserialize(source);
        }
    }
}
