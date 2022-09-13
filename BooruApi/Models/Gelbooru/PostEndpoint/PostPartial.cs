using System.Text.Json.Serialization;

namespace BooruApi.Models.Gelbooru.PostEndpoint
{
    public partial class Post
    {
        [JsonIgnore]
        public string PostUrl { get; set; }
    }
}
