using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LayeredStorageNoAlloc.Generic
{
    public class SingleLayerStorageNoAlloc<TL1Key, TValue> where TL1Key : IEquatable<TL1Key>
    {
        private readonly IList<SingleLayerEntry<TL1Key, TValue>> Storage = Unsafe.As<IList<SingleLayerEntry<TL1Key, TValue>>>(ArrayList.Synchronized(new List<SingleLayerEntry<TL1Key, TValue>>()));
        public virtual TValue this[TL1Key index]
        {
            get
            {
                if (TryGetValue(index, out var value))
                {
                    return value;
                }
                return default;
            }

            set
            {
                AddOrUpdate(index, value);
            }
        }

        public bool TryGetValue(TL1Key key1, out TValue value)
        {
            for (int i = 0; i < Storage.Count; i++)
            {
                var currentValue = Storage[i];

                if (currentValue.K1.Equals(key1))
                {
                    value = currentValue.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public void AddOrUpdate(TL1Key key1, TValue value)
        {
            for (int i = 0; i < Storage.Count; i++)
            {
                var currentValue = Storage[i];

                if (currentValue.K1.Equals(key1))
                {
                    currentValue.Value = value;
                    Storage[i] = currentValue;
                    return;
                }

            }

            Storage.Add(new SingleLayerEntry<TL1Key, TValue>()
            {
                K1 = key1,
                Value = value,
            });
        }

        public bool TryRemove(TL1Key key1, out TValue value)
        {
            for (int i = 0; i < Storage.Count; i++)
            {
                var currentValue = Storage[i];

                if (currentValue.K1.Equals(key1))
                {
                    value = currentValue.Value;
                    Storage.RemoveAt(i);
                    return true;
                }

            }

            value = default;
            return false;
        }

        public IList<SingleLayerEntry<TL1Key,TValue>> ToList()
        {
            return Storage;
        }
    }
}
