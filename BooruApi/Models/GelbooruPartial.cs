using System.Text.Json.Serialization;

namespace BooruApi.Models
{
    public partial class Post
    {
        [JsonIgnore]
        public string PostUrl { get; set; }
    }
}
