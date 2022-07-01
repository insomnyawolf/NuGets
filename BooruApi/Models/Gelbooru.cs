using System.Text.Json.Serialization;

namespace BooruApi.Models
{
    public class Attributes
    {

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class Post
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }

        [JsonPropertyName("directory")]
        public string Directory { get; set; }

        [JsonPropertyName("image")]
        public string Image { get; set; }

        [JsonPropertyName("rating")]
        public string Rating { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("change")]
        public int Change { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("creator_id")]
        public int CreatorId { get; set; }

        [JsonPropertyName("parent_id")]
        public int ParentId { get; set; }

        [JsonPropertyName("sample")]
        public int Sample { get; set; }

        [JsonPropertyName("preview_height")]
        public int PreviewHeight { get; set; }

        [JsonPropertyName("preview_width")]
        public int PreviewWidth { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("has_notes")]
        public string HasNotes { get; set; }

        [JsonPropertyName("has_comments")]
        public string HasComments { get; set; }

        [JsonPropertyName("file_url")]
        public string FileUrl { get; set; }

        [JsonPropertyName("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonPropertyName("sample_url")]
        public string SampleUrl { get; set; }

        [JsonPropertyName("sample_height")]
        public int SampleHeight { get; set; }

        [JsonPropertyName("sample_width")]
        public int SampleWidth { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("post_locked")]
        public int PostLocked { get; set; }

        [JsonPropertyName("has_children")]
        public string HasChildren { get; set; }
    }

    public class ApiResponse
    {

        [JsonPropertyName("@attributes")]
        public Attributes Attributes { get; set; }

        [JsonPropertyName("post")]
        public IList<Post> Post { get; set; }
    }


}
