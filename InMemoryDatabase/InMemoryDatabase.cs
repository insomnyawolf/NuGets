using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InMemoryDatabase
{
#warning THIS PROBABLY ISN'T THREAD SAFE YET
    public class InMemoryDatabase<T> where T : class
    {
        private readonly MemoryPool<DatabaseEntry<T>> DatabaseEntryPool = MemoryPool<DatabaseEntry<T>>.Shared;
        private readonly MemoryPool<T> ResultPool = MemoryPool<T>.Shared;

        private List<DatabaseEntry<T>> Data;

        private Stream? PersistanceStream;
        private readonly FileStream? PersistanceFileStream;

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions()
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            ReadCommentHandling = JsonCommentHandling.Skip,
            Encoder = JavaScriptEncoder.Default,
        };

        public InMemoryDatabase(string path) : this()
        {
            var FileStreamOptions = new FileStreamOptions
            {
                Access = FileAccess.ReadWrite,
                Mode = FileMode.OpenOrCreate,
                Share = FileShare.Read,
                Options = FileOptions.SequentialScan | FileOptions.WriteThrough,
                BufferSize = 4096,
            };

            PersistanceFileStream = File.Open(path, FileStreamOptions);
            Init(PersistanceStream: PersistanceFileStream);
        }

        public InMemoryDatabase(Stream? PersistanceStream)
        {
            Init(PersistanceStream: PersistanceStream);
        }

        public InMemoryDatabase()
        {
            Init(PersistanceStream: null);
        }

        private void Init(Stream PersistanceStream)
        {
            this.PersistanceStream = PersistanceStream;

            if (PersistanceStream is not null)
            {
                // Only try to load if there's a plate to load from
                Load();
            }

            if (Data is null)
            {
                // Then assing a default value
                Data = new List<DatabaseEntry<T>>();
            }
        }

        #region CRUD

        public T[] Find(Func<T, bool> filter)
        {
            var rowsAffected = 0;
            for (int i = 0; i < Data.Count; i++)
            {
                var currentData = Data[i];
                if (filter(currentData.Value))
                {
                    currentData.Marked = true;
                    rowsAffected++;
                }
            }

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

            return span[..rowsAffected].ToArray();
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

#warning improve the update methods?
        public int UpdateWhere(Func<T, bool> filter, Action<T> updateAction)
        {
            var rowsAffected = 0;

            for (int i = 0; i < Data.Count; i++)
            {
                var currentData = Data[i];
                if (filter(currentData.Value))
                {
                    updateAction(currentData.Value);
                    rowsAffected++;
                }
            }

            return rowsAffected;
        }

        public int UpdateWhere(Func<T, bool> filter, Func<T, T> updateAction)
        {
            var rowsAffected = 0;

            for (int i = 0; i < Data.Count; i++)
            {
                var currentData = Data[i];
                if (filter(currentData.Value))
                {
                    currentData.Value = updateAction(currentData.Value);
                    rowsAffected++;
                }
            }

            return rowsAffected;
        }

        public int DeleteWhere(Func<T, bool> filter)
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
            if (PersistanceStream is null)
            {
                throw new ArgumentNullException(nameof(PersistanceStream));
            }
            PersistanceFileStream?.SetLength(0);
            Export(PersistanceFileStream);
            PersistanceStream.Flush();
        }

        public void Load()
        {
            if (PersistanceStream is null)
            {
                throw new ArgumentNullException(nameof(PersistanceStream));
            }

            if (PersistanceFileStream.Length > 0)
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
        //
        // Total Hours Spent: 22
        public void Export(Stream stream)
        {
            JsonSerializer.Serialize(stream, Data, JsonSerializerOptions);
        }

        public string Export()
        {
            return JsonSerializer.Serialize(Data, JsonSerializerOptions);
        }

        public void Import(Stream stream)
        {
            Data = JsonSerializer.Deserialize<List<DatabaseEntry<T>>>(stream, JsonSerializerOptions);
        }
    }
    #endregion Persistance
}