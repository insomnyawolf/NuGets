using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SaucenaoApi.Models
{
    // Generated with https://app.quicktype.io/?l=csharp
    // Modified by hand
    // SaucenaoResponse myDeserializedClass = JsonSerializer.Deserialize<SaucenaoResponse>(myJsonResponse);
    using System;
    using System.Collections.Generic;
    using System.Text.Json;

    public partial class SaucenaoResponse
    {
        [JsonPropertyName("header")]
        public TemperaturesHeader Header { get; set; }

        [JsonPropertyName("results")]
        public List<Result> Results { get; set; }
    }

    public partial class TemperaturesHeader
    {
        [JsonPropertyName("user_id")]
        [JsonConverter(typeof(ParseNumberConverter))]
        public long UserId { get; set; }

        [JsonPropertyName("account_type")]
        [JsonConverter(typeof(ParseNumberConverter))]
        public long AccountType { get; set; }

        [JsonPropertyName("short_limit")]
        [JsonConverter(typeof(ParseNumberConverter))]
        public long ShortLimit { get; set; }

        [JsonPropertyName("long_limit")]
        [JsonConverter(typeof(ParseNumberConverter))]
        public long LongLimit { get; set; }

        [JsonPropertyName("long_remaining")]
        public long LongRemaining { get; set; }

        [JsonPropertyName("short_remaining")]
        public long ShortRemaining { get; set; }

        [JsonPropertyName("status")]
        public long Status { get; set; }

        [JsonPropertyName("results_requested")]
        [JsonConverter(typeof(ParseNumberConverter))]
        public long ResultsRequested { get; set; }

        [JsonPropertyName("index")]
        public Dictionary<string, Index> Index { get; set; }

        [JsonPropertyName("search_depth")]
        [JsonConverter(typeof(ParseNumberConverter))]
        public long SearchDepth { get; set; }

        [JsonPropertyName("minimum_similarity")]
        public double MinimumSimilarity { get; set; }

        [JsonPropertyName("query_image_display")]
        public string QueryImageDisplay { get; set; }

        [JsonPropertyName("query_image")]
        public string QueryImage { get; set; }

        [JsonPropertyName("results_returned")]
        public long ResultsReturned { get; set; }
    }

    public partial class Index
    {
        [JsonPropertyName("status")]
        public long Status { get; set; }

        [JsonPropertyName("parent_id")]
        public long ParentId { get; set; }

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("results")]
        public long? Results { get; set; }
    }

    public partial class Result
    {
        [JsonPropertyName("header")]
        public ResultHeader Header { get; set; }

        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonPropertyName("ext_urls")]
        public List<Uri> ExtUrls { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("pixiv_id")]
        public long? PixivId { get; set; }

        [JsonPropertyName("member_name")]
        public string MemberName { get; set; }

        [JsonPropertyName("member_id")]
        public long? MemberId { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("anidb_aid")]
        public long? AnidbAid { get; set; }

        [JsonPropertyName("part")]
        public string Part { get; set; }

        [JsonPropertyName("year")]
        public string Year { get; set; }

        [JsonPropertyName("est_time")]
        public string EstTime { get; set; }

        [JsonPropertyName("imdb_id")]
        public string ImdbId { get; set; }

        [JsonPropertyName("sankaku_id")]
        public long? SankakuId { get; set; }

        [JsonPropertyName("creator")]
        public string Creator { get; set; }

        [JsonPropertyName("material")]
        public string Material { get; set; }

        [JsonPropertyName("characters")]
        public string Characters { get; set; }

        [JsonPropertyName("da_id")]
        [JsonConverter(typeof(ParseNumberConverter))]
        public long? DaId { get; set; }

        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; }

        [JsonPropertyName("author_url")]
        public Uri AuthorUrl { get; set; }
    }

    public partial class ResultHeader
    {
        [JsonPropertyName("similarity")]
        public string Similarity { get; set; }

        [JsonPropertyName("thumbnail")]
        public Uri Thumbnail { get; set; }

        [JsonPropertyName("index_id")]
        public long IndexId { get; set; }

        [JsonPropertyName("index_name")]
        public string IndexName { get; set; }

        [JsonPropertyName("dupes")]
        public long Dupes { get; set; }
    }

    public class ParseStringConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(string) == typeToConvert;
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                return reader.TryGetInt64(out long l) ?
                    l.ToString() :
                    reader.GetDouble().ToString();
            }
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return document.RootElement.Clone().ToString();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    public class ParseNumberConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(long) == typeToConvert || typeof(long?) == typeToConvert;
        }
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (long.TryParse(reader.GetString(), out long res))
                {
                    return res;
                }
                return null;
            }
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out long l))
                {
                    return l;
                }
                return null;
            }
            using (JsonDocument document = JsonDocument.ParseValue(ref reader))
            {
                return document.RootElement.Clone().ToString();
            }
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
