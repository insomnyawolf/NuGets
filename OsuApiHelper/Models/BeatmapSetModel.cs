using System.Text.Json.Serialization;

namespace OsuApiHelper
{
    public class BeatmapsetCompact
    {
        public string artist { get; set; }
        public string artist_unicode { get; set; }
        public Covers covers { get; set; }
        public string creator { get; set; }
        public int favourite_count { get; set; }
        public int id { get; set; }
        public bool nsfw { get; set; }
        public int play_count { get; set; }
        public string preview_url { get; set; }
        public string source { get; set; }
        public string status { get; set; }
        public string title { get; set; }
        public string title_unicode { get; set; }
        public int user_id { get; set; }
        public bool video { get; set; }
        public Beatmap[] beatmaps { get; set; }
        public object converts { get; set; }
        public object current_user_attributes { get; set; }
        public string description { get; set; }
        public object discussions { get; set; }
        public object events { get; set; }
        public string genre { get; set; }
        public bool has_favourited { get; set; }
        public string language { get; set; }
        public object nominations { get; set; }
        public object ratings { get; set; }
        public object recent_favourites { get; set; }
        public object related_users { get; set; }
        public string user { get; set; }
    }

    public class Beatmapset : BeatmapsetCompact
    {
        [JsonPropertyName("availability.download_disabled")]
        public string availabilitydownload_disabled { get; set; }
        [JsonPropertyName("availability.more_information")]
        public string availabilitymore_information { get; set; }
        public float bpm { get; set; }
        public bool can_be_hyped { get; set; }
        public string creator { get; set; }
        public bool discussion_enabled { get; set; }
        public bool discussion_locked { get; set; }
        [JsonPropertyName("hype.current")]
        public string hypecurrent { get; set; }
        [JsonPropertyName("hype.required")]
        public string hyperequired { get; set; }
        public bool is_scoreable { get; set; }
#warning convert to timestamp
        public string last_updated { get; set; }
        public string legacy_thread_url { get; set; }
        [JsonPropertyName("nominations.current")]
        public string nominationscurrent { get; set; }
        [JsonPropertyName("nominations.required")]
        public string nominationsrequired { get; set; }
        public RankStatus ranked { get; set; }
#warning convert to timestamp
        public string ranked_date { get; set; }
        public string source { get; set; }
        public bool storyboard { get; set; }
#warning convert to timestamp
        public string submitted_date { get; set; }
        public string tags { get; set; }
    }

    public class Covers
    {
        public string cover { get; set; }
        [JsonPropertyName("cover@2x")]
        public string cover2x { get; set; }
        public string card { get; set; }
        [JsonPropertyName("card@2x")]
        public string card2x { get; set; }
        public string list { get; set; }
        [JsonPropertyName("list@2x")]
        public string list2x { get; set; }
        public string slimcover { get; set; }
        [JsonPropertyName("slimcover@2x")]
        public string slimcover2x { get; set; }
    }
}
