using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Extensions
{
    public static class Object
    {
        public static TClass[] AsArray<TClass>(this TClass item)
        {
            return new TClass[] { item };
        }

        public static MemoryStream ToStream<TClass>(this TClass source)
        {
            var memoryStream = new MemoryStream();
            return (MemoryStream)source.ToStream(memoryStream);
        }

        public static Stream ToStream<TClass>(this TClass source, Stream stream)
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

            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;

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
