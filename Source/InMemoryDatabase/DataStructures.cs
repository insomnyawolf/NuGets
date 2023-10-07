#define CustomConverter
using System.Text.Json.Serialization;

namespace InMemoryDatabase
{
#if CustomConverter
    [JsonConverter(typeof(DatabaseEntryConverter))]
#endif
    public class DatabaseEntry<T> where T : class
    {
#if !CustomConverter
        [JsonPropertyName("V")]
#endif
        public T? Value { get; set; }
    }
}
