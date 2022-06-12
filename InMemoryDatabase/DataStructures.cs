//#define CustomConverter
//#define OptimizeDefaultConverter
using System.Text.Json.Serialization;

namespace InMemoryDatabase
{
#if CustomConverter
    [JsonConverter(typeof(DatabaseEntryConverter))]
#endif
    public class DatabaseEntry<T> where T : class
    {
#if OptimizeDefaultConverter
        [JsonPropertyName("V")]
#endif
        public T? Value { get; set; }
#if OptimizeDefaultConverter
        [JsonIgnore]
#endif
        public bool Marked { get; set; }
    }
}
