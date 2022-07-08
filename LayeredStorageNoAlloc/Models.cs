namespace LayeredStorageNoAlloc
{
    public class DualLayerEntry<TL1Key, TL2Key, TValue> : SingleLayerEntry<TL1Key, TValue>
    {
        public TL2Key K2 { get; set; }
    }

    public class SingleLayerEntry<TL1Key, TValue>
    {
        public TL1Key K1 { get; set; }
        public TValue Value { get; set; }
    }
}
