using System;
using System.Collections;
using System.Collections.Generic;

namespace LayeredStorageNoAlloc.Generic
{
    public class DoubleLayerStorageNoAlloc<TL1Key, TL2Key, TValue> where TL1Key : IEquatable<TL1Key> where TL2Key : IEquatable<TL2Key> where TValue : class
    {
        private readonly ArrayList Storage = ArrayList.Synchronized(new ArrayList());

        public bool TryGetValue(TL1Key key1, TL2Key key2, out TValue value)
        {
            for (int i = 0; i < Storage.Count; i++)
            {
                if (Storage[i] is DualLayerEntry<TL1Key, TL2Key, TValue> currentValue)
                {
                    if (currentValue.K1.Equals(key1) && currentValue.K2.Equals(key2))
                    {
                        value = currentValue.Value;
                        return true;
                    }
                }
                else
                {
                    // Should NEVER Execute this
                    throw new Exception();
                }
            }

            value = default;
            return false;
        }

        public List<Tuple<TL2Key, TValue>> GetAll(TL1Key key)
        {
            var result = new List<Tuple<TL2Key, TValue>>();
            for (int i = 0; i < Storage.Count; i++)
            {
                if (Storage[i] is DualLayerEntry<TL1Key, TL2Key, TValue> currentValue)
                {
                    if (currentValue.K1.Equals(key))
                    {
                        result.Add(new Tuple<TL2Key, TValue>(currentValue.K2, currentValue.Value));
                    }
                }
                else
                {
                    // Should NEVER Execute this
                    throw new Exception();
                }
            }
            return result;
        }


        public void AddOrUpdate(TL1Key key1, TL2Key key2, TValue value)
        {
            for (int i = 0; i < Storage.Count; i++)
            {
                if (Storage[i] is DualLayerEntry<TL1Key, TL2Key, TValue> currentValue)
                {
                    if (currentValue.K1.Equals(key1) && currentValue.K2.Equals(key2))
                    {
                        currentValue.Value = value;
                        Storage[i] = currentValue;
                        return;
                    }
                }
                else
                {
                    // Should NEVER Execute this
                    throw new Exception();
                }
            }

            Storage.Add(new DualLayerEntry<TL1Key, TL2Key, TValue>()
            {
                K1 = key1,
                K2 = key2,
                Value = value,
            });
        }

        public bool TryRemove(TL1Key key1, TL2Key key2, out TValue value)
        {
            for (int i = 0; i < Storage.Count; i++)
            {
                if (Storage[i] is DualLayerEntry<TL1Key, TL2Key, TValue> currentValue)
                {
                    if (currentValue.K1.Equals(key1) && currentValue.K2.Equals(key2))
                    {
                        value = currentValue.Value;
                        Storage.RemoveAt(i);
                        return true;
                    }
                }
                else
                {
                    // Should NEVER Execute this
                    throw new Exception();
                }
            }

            value = default;
            return false;
        }
    }
}
