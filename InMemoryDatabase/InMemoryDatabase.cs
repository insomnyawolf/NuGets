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
#warning THIS PROBABLY ISN'T THREAD SAFE YET
    public class InMemoryDatabase<T> where T : class
    {
        //private readonly MemoryPool<DatabaseEntry<T>> DatabaseEntryPool = MemoryPool<DatabaseEntry<T>>.Shared;
        private readonly MemoryPool<T> ResultPool = MemoryPool<T>.Shared;

        private List<DatabaseEntry<T>> Data;

        private Stream? PersistanceStream;

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.Default,
        };

        public bool IsCompressed { get; }

        public InMemoryDatabase(string path, bool compressed = false) : this(File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read), compressed)
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

        
        public InMemoryDatabase(Stream? PersistanceStream = null, bool compressed = false)
        {
            IsCompressed = compressed;
            Init(PersistanceStream: PersistanceStream);
        }

        private void Init(Stream? PersistanceStream)
        {
            this.PersistanceStream = PersistanceStream;

            if (PersistanceStream != null)
            {
                // Only try to load if there's a plate to load from
                Load();
            }

            if (Data is null)
            {
                // Then assing a default value
                Data = ArrayList.Synchronized(new List<DatabaseEntry<T>>()) as List<DatabaseEntry<T>>;
            }
        }

        #region CRUD

#warning am i overcomplicating myself by not using a simple List<T> ?
        public T[] Find(Func<T?, bool> filter)
        {
            var rowsAffected = 0;

            Parallel.For(0, Data.Count, () => 0, (i, loop, subtotal) =>
            {
                var currentData = Data[i];
                if (filter(currentData.Value))
                {
                    currentData.Marked = true;
                    subtotal++;
                }
                return subtotal;
            },
                (x) => Interlocked.Add(ref rowsAffected, x)
            );

            using var memoryOwner = ResultPool.Rent(rowsAffected);
            var span = memoryOwner.Memory.Span;

            var currentIndex = 0;

            for (int i = 0; i < Data.Count; i++)
            {
                var currentData = Data[i];
                if (currentData.Marked)
                {
                    currentData.Marked = false;
                    span[currentIndex] = currentData.Value;
                    currentIndex++;
                }
            }

            return span/*[..rowsAffected]*/.ToArray();
        }

        //public List<T?> FindSimple(Func<T?, bool> filter)
        //{
        //    var results = new List<T?>();
        //    for (int i = 0; i < Data.Count; i++)
        //    {
        //        var currentData = Data[i];
        //        if (filter(currentData.Value))
        //        {
        //            results.Add(currentData.Value);
        //        }
        //    }
        //    return results;
        //}

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

        public int CountWhere(Func<T?, bool> filter)
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

#warning improve the update methods?
        public int UpdateWhere(Func<T?, bool> filter, Action<T?> updateAction)
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

        public int DeleteWhere(Func<T?, bool> filter)
        {
            var rowsAffected = 0;

            var index = Data.Count;

            while (Data.Count > 0)
            {
                index--;
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

        public void Load()
        {
            if (PersistanceStream == null)
            {
                throw new ArgumentNullException(nameof(PersistanceStream));
            }

            if (PersistanceStream.Length > 0)
            {
                Import(PersistanceStream);
            }
        }
#warning PAIN

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

        public void Import(Stream stream)
        {
            var targetStream = IsCompressed ? new GZipStream(stream, CompressionMode.Decompress) : stream;
            Data = ArrayList.Synchronized(JsonSerializer.Deserialize<List<DatabaseEntry<T>>>(targetStream, JsonSerializerOptions)) as List<DatabaseEntry<T>>;
        }
    }
    #endregion Persistance
}