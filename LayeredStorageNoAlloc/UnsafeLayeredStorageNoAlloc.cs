using System;
using System.Collections.Generic;
namespace LayeredStorageNoAlloc
{
    public class UnsafeLayeredStorageNoAlloc<TL1Key, TValue> : List<SingleLayerEntry<TL1Key, TValue>>
    {
        public bool TryGetValue(TL1Key key1, out TValue value)
        {
            for (int i = 0; i < Count; i++)
            {
                var currentValue = this[i];

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
            for (int i = 0; i < Count; i++)
            {
                var currentValue = this[i];

                if (currentValue.K1.Equals(key1))
                {
                    currentValue.Value = value;
                    this[i] = currentValue;
                    return;
                }
            }

            Add(new SingleLayerEntry<TL1Key, TValue>()
            {
                K1 = key1,
                Value = value,
            });
        }

        public bool TryRemove(TL1Key key1)
        {
            for (int i = 0; i < Count; i++)
            {
                var currentValue = this[i];

                if (currentValue.K1.Equals(key1))
                {
                    RemoveAt(i);
                    return true;
                }

            }
            return false;
        }
    }
}
