using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace InMemoryDatabase
{
    public class InMemoryDatabase<T> where T : class
    {
        public bool IsCompressed { get; }
        private readonly List<DatabaseEntry<T>> Data;
        private readonly Stream PersistanceStream;

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.Default,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            IncludeFields = true,
            IgnoreReadOnlyFields = false,
            IgnoreReadOnlyProperties = false,
            NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.AllowNamedFloatingPointLiterals,
            PropertyNameCaseInsensitive = false,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement,
        };


        public InMemoryDatabase(string path, bool IsCompressed = false) : this(File.Open(path, mode: FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read), IsCompressed)
        {
            //var fileStreamOptions = new FileStreamOptions
            //{
            //    Access = FileAccess.ReadWrite,
            //    Mode = FileMode.OpenOrCreate,
            //    Share = FileShare.Read,
            //    Options = FileOptions.SequentialScan | FileOptions.WriteThrough,
            //    BufferSize = 4096,
            //};
        }

        public InMemoryDatabase(Stream PersistanceStream = null, bool IsCompressed = false)
        {
            this.IsCompressed = IsCompressed;
            this.PersistanceStream = PersistanceStream;

            if (PersistanceStream != null)
            {
                // Only try to load if there's a plate to load from
                Data = Load();
            }

            // Then assing a default value if no data is available
            Data ??= (List<DatabaseEntry<T>>)ArrayList.Synchronized(new List<DatabaseEntry<T>>());
        }

        #region CRUD

        public List<T> Find(Func<T, bool> filter)
        {
            var rowsAffected = 0;

            var result = new List<T>();

            Parallel.For(fromInclusive:0, toExclusive: Data.Count, localInit: () => 0, body: (i, loop, subtotal) =>
            {
                var currentData = Data[i];
                if (filter(currentData.Value))
                {
                    result.Add(currentData.Value);
                    subtotal++;
                }
                return subtotal;
            } ,localFinally: (x) => Interlocked.Add(ref rowsAffected, x)
            );

            return result;
        }

        public void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            Data.Add(new DatabaseEntry<T>()
            {
                Value = item
            });
        }

        public void Add(IList<T> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                Add(items[i]);
            }
        }

        public int CountWhere(Func<T, bool> filter)
        {
            var rowsAffected = 0;

            Parallel.For(0, Data.Count, () => 0, (i, loop, subtotal) =>
            {
                var currentData = Data[i];
                if (filter(currentData.Value))
                {
                    subtotal++;
                }
                return subtotal;
            },
                (x) => Interlocked.Add(ref rowsAffected, x)
            );

            return rowsAffected;
        }

        public int UpdateWhere(Func<T, bool> filter, Action<T> updateAction)
        {
            var rowsAffected = 0;

            Parallel.For(0, Data.Count, () => 0, (i, loop, subtotal) =>
            {
                var currentData = Data[i];
                if (filter(currentData.Value))
                {
                    updateAction(currentData.Value);
                    subtotal++;
                }

                return subtotal;
            },
                (x) => Interlocked.Add(ref rowsAffected, x)
            );

            return rowsAffected;
        }

        public int DeleteWhere(Func<T, bool> filter)
        {
            var rowsAffected = 0;

            // Doing multithreading in this is insanity

            for (int index = Data.Count -1 ; index > -1; index--)
            {
                var currentData = Data[index];
                if (filter(currentData.Value))
                {
                    Data.RemoveAt(index);
                    rowsAffected++;
                }
            }

            return rowsAffected;
        }

        #endregion CRUD

        #region Persistance
        public void Save()
        {
            if (PersistanceStream == null)
            {
                throw new ArgumentNullException(nameof(PersistanceStream));
            }

            PersistanceStream.SetLength(0);
            Export(PersistanceStream);
            PersistanceStream.Flush();
        }

        public List<DatabaseEntry<T>> Load()
        {
            if (PersistanceStream == null)
            {
                throw new ArgumentNullException(nameof(PersistanceStream));
            }

            if (PersistanceStream.Length == 0)
            {
                return null;
            }

            return Import(PersistanceStream);
        }

        // This could be better and don't have bullshit in between but im tired
        // Either i have good performance and bullshit
        // Or the data clean but a shitty performance that takes years to do anything 
        // Trying to optimize this i've learnt protobufs deprecated shit binary formatter and others wich DIDN'T WORK EITHER
        // At this point i'd rather write everything by hand...
        // Computers were a mistake
        // 
        // Total Hours Spent: 22
        public string Export()
        {
            return JsonSerializer.Serialize(Data, JsonSerializerOptions);
        }

        public void Export(Stream stream)
        {
            var targetStream = IsCompressed ? new GZipStream(stream, CompressionMode.Compress) : stream;
            JsonSerializer.Serialize(targetStream, Data, JsonSerializerOptions);
        }

        public List<DatabaseEntry<T>> Import(Stream stream)
        {
            var targetStream = IsCompressed ? new GZipStream(stream, CompressionMode.Decompress) : stream;
            return JsonSerializer.Deserialize<List<DatabaseEntry<T>>>(targetStream, JsonSerializerOptions);
        }
    }
    #endregion Persistance
}