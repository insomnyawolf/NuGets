using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BooruApi.Models.Gelbooru.TagEndpoint
{
    public class ApiResponseTag
    {
        [JsonPropertyName("@attributes")]
        public Attributes Attributes { get; set; }

        [JsonPropertyName("tag")]
        public List<Tag> Tag { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("ambiguous")]
        public int Ambiguous { get; set; }
    }
}
