using System.Text.Json.Serialization;

namespace BooruApi.Models.Gelbooru
{
    public class Attributes
    {

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        public bool PreviousPageAvailable()
        {
            return Offset == 0;
        }

        public bool IsNextPageAvailable()
        {
            return Offset + Limit < Count;
        }
    }

    public class AutoCompleteResponse
    {

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("post_count")]
        public string PostCount { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }
    }
}
