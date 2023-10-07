using System.Text.Json.Serialization;

namespace OsuApiHelper
{
    public class Failtimes
    {
        public int[] exit { get; set; }
        public int[] fail { get; set; }
    }

    public class BeatmapCompact
    {
        public int beatmapset_id { get; set; }
        public float difficulty_rating { get; set; }
        public int id { get; set; }
#warning Change to enum //https://osu.ppy.sh/docs/index.html?bash#gamemode
        public string mode { get; set; }
        public string status { get; set; }
        public int total_length { get; set; }
        public int user_id { get; set; }
        /// <summary>
        /// Difficulty name
        /// </summary>
        public string version { get; set; }
        public Beatmapset beatmapset { get; set; }
        public string checksum { get; set; }
        public Failtimes failtimes { get; set; }
        public int max_combo { get; set; }
    }

    public class Beatmap : BeatmapCompact
    {
        public float accuracy { get; set; }
        public float ar { get; set; }
        public float? bpm { get; set; }
        public bool convert { get; set; }
        public int count_circles { get; set; }
        public int count_sliders { get; set; }
        public int count_spinners { get; set; }
        public float cs { get; set; }
#warning change to timestamp
        public string deleted_at { get; set; }
        public float drain { get; set; }
        public int hit_length { get; set; }
        public bool is_scoreable { get; set; }
#warning change to timestamp
        public string last_updated { get; set; }
        public int mode_int { get; set; }
        public int passcount { get; set; }
        public int playcount { get; set; }
        public RankStatus ranked { get; set; }
    }

    public enum RankStatus
    {
        graveyard = -2,
        wip = -1,
        pending = 0,
        ranked = 1,
        approved = 2,
        qualified = 3,
        loved = 4,
    }
}
