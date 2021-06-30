using System;
using System.Collections;

namespace LayeredStorageNoAlloc
{
    public class SingleLayerStorageNoAlloc<TL1Key, TValue> where TL1Key : IEquatable<TL1Key> where TValue : class
    {
        private readonly ArrayList Storage = ArrayList.Synchronized(new ArrayList());

        public bool TryGetValue(TL1Key key1, out TValue value)
        {
            for (int i = 0; i < Storage.Count; i++)
            {
                if (Storage[i] is SingleLayerEntry<TL1Key, TValue> currentValue)
                {
                    if (currentValue.K1.Equals(key1))
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

        // Makes no sense
        //public List<TValue> GetAll(TL1Key key)
        //{
        //    var result = new List<TValue>();
        //    for (int i = 0; i < Storage.Count; i++)
        //    {
        //        if (Storage[i] is SingleLayerEntry<TL1Key, TValue> currentValue)
        //        {
        //            if (currentValue.K1.Equals(key))
        //            {
        //                result.Add(currentValue.Value);
        //            }
        //        }
        //        else
        //        {
        //            // Should NEVER Execute this
        //            throw new Exception();
        //        }
        //    }
        //    return result;
        //}


        public void AddOrUpdate(TL1Key key1, TValue value)
        {
            for (int i = 0; i < Storage.Count; i++)
            {
                if (Storage[i] is SingleLayerEntry<TL1Key, TValue> currentValue)
                {
                    if (currentValue.K1.Equals(key1))
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
                if (Storage[i] is SingleLayerEntry<TL1Key, TValue> currentValue)
                {
                    if (currentValue.K1.Equals(key1))
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
