using System.Buffers;
using System.Text.Json;

namespace InMemoryDatabase
{
#warning RHIS PROBABLY ISN'T THREAD SAFE YET
    public class InMemoryDatabase<T>
    {
        private readonly MemoryPool<DatabaseEntry<T>> DatabaseEntryPool = MemoryPool<DatabaseEntry<T>>.Shared;
        private readonly MemoryPool<T> ResultPool = MemoryPool<T>.Shared;

        private List<DatabaseEntry<T>> Data;

        private Stream? PersistanceStream;
        private FileStream? PersistanceFile;

        public InMemoryDatabase(string path) : this()
        {
            this.PersistanceFile = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            Init(PersistanceStream: PersistanceFile);
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
                try
                {
                    Load();
                }
                catch (JsonException ex)
                {
                    // Ignore errors if nothing could be loaded during database creation

                }
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
                if (filter(currentData.Data))
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
                    span[currentIndex] = currentData.Data;
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
                Data = item
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
                if (filter(currentData.Data))
                {
                    updateAction(currentData.Data);
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
                if (filter(currentData.Data))
                {
                    currentData.Data = updateAction(currentData.Data);
                    rowsAffected++;
                }
            }

            return rowsAffected;
        }

        public int DeleteWhere(Func<T, bool> filter)
        {
            var rowsAffected = 0;
            for (int i = 0; i < Data.Count; i++)
            {
                var currentData = Data[i];
                if (filter(currentData.Data))
                {
                    currentData.Marked = true;
                    rowsAffected++;
                }
            }

            using (var memoryOwner = DatabaseEntryPool.Rent(rowsAffected))
            {
                var span = memoryOwner.Memory.Span;

                var correntIndex = 0;

                for (int i = 0; i < Data.Count; i++)
                {
                    var currentData = Data[i];
                    if (currentData.Marked)
                    {
                        currentData.Marked = false;
                        span[correntIndex] = currentData;
                        correntIndex++;
                    }
                }

                for (int i = 0; i < rowsAffected; i++)
                {
                    Data.Remove(span[i]);
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

            PersistanceFile?.SetLength(0);
            Export(PersistanceStream);
        }

        public void Load()
        {
            if (PersistanceStream is null)
            {
                throw new ArgumentNullException(nameof(PersistanceStream));
            }
            Import(PersistanceStream);
        }

        public void Export(Stream stream)
        {
            JsonSerializer.Serialize(stream, Data);
        }

        public string Export()
        {
            return JsonSerializer.Serialize(Data);
        }

        public void Import(Stream stream)
        {
            Data = JsonSerializer.Deserialize<List<DatabaseEntry<T>>>(stream);
        }
    }
    #endregion Persistance

    internal class DatabaseEntry<T>
    {
        public T? Data { get; set; }
        public bool Marked { get; set; }
    }
}