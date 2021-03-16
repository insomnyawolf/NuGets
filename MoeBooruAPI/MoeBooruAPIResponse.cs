using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MoeBooruApi
{
    public partial class MoeBooruAPIResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("created_at")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("creator_id")]
        public long? CreatorId { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("change")]
        public long Change { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("score")]
        public long Score { get; set; }

        [JsonPropertyName("md5")]
        public string Md5 { get; set; }

        [JsonPropertyName("file_size")]
        public long FileSize { get; set; }

        [JsonPropertyName("file_url")]
        public Uri FileUrl { get; set; }

        [JsonPropertyName("is_shown_in_index")]
        public bool IsShownInIndex { get; set; }

        [JsonPropertyName("preview_url")]
        public Uri PreviewUrl { get; set; }

        [JsonPropertyName("preview_width")]
        public long PreviewWidth { get; set; }

        [JsonPropertyName("preview_height")]
        public long PreviewHeight { get; set; }

        [JsonPropertyName("actual_preview_width")]
        public long ActualPreviewWidth { get; set; }

        [JsonPropertyName("actual_preview_height")]
        public long ActualPreviewHeight { get; set; }

        [JsonPropertyName("sample_url")]
        public Uri SampleUrl { get; set; }

        [JsonPropertyName("sample_width")]
        public long SampleWidth { get; set; }

        [JsonPropertyName("sample_height")]
        public long SampleHeight { get; set; }

        [JsonPropertyName("sample_file_size")]
        public long SampleFileSize { get; set; }

        [JsonPropertyName("jpeg_url")]
        public Uri JpegUrl { get; set; }

        [JsonPropertyName("jpeg_width")]
        public long JpegWidth { get; set; }

        [JsonPropertyName("jpeg_height")]
        public long JpegHeight { get; set; }

        [JsonPropertyName("jpeg_file_size")]
        public long JpegFileSize { get; set; }

        [JsonPropertyName("rating")]
        [JsonConverter(typeof(EnumConverter<MoeBooruRating>))]
        public MoeBooruRating Rating { get; set; }

        [JsonPropertyName("has_children")]
        public bool HasChildren { get; set; }

        [JsonPropertyName("parent_id")]
        public object ParentId { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(EnumConverter<MoeBooruStatus>))]
        public MoeBooruStatus Status { get; set; }

        [JsonPropertyName("width")]
        public long Width { get; set; }

        [JsonPropertyName("height")]
        public long Height { get; set; }

        [JsonPropertyName("is_held")]
        public bool IsHeld { get; set; }

        [JsonPropertyName("frames_pending_string")]
        public string FramesPendingString { get; set; }

        [JsonPropertyName("frames_pending")]
        public List<object> FramesPending { get; set; }

        [JsonPropertyName("frames_string")]
        public string FramesString { get; set; }

        [JsonPropertyName("frames")]
        public List<object> Frames { get; set; }
    }

    public partial class MoeBooruAPIResponse
    {
        public static List<MoeBooruAPIResponse> FromJson(string json) => JsonSerializer.Deserialize<List<MoeBooruAPIResponse>>(json/*, Converter.Settings*/);
    }

    //internal static class Converter
    //{
    //    public static readonly JsonSerializerOptions Settings = new JsonSerializerOptions
    //    {
    //        //MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //        //DateParseHandling = DateParseHandling.None,
    //        //Converters =
    //        //{
    //        //    RatingConverter.Singleton,
    //        //    StatusConverter.Singleton,
    //        //    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
    //        //},
    //    };
    //}

    internal class EnumConverter<T> : JsonConverter<T> where T : Enum
    {
        public override bool CanConvert(Type t) => t == typeof(T) || t == typeof(T?);

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return default;
            }

            var type = typeof(T);

            var strValue = reader.GetString();

            var values = (T[])Enum.GetValues(type);

            for (int i = 0; i < values.Length; i++)
            {
                T val = values[i];
                if (strValue == val.ToStringCustom())
                {
                    return val;
                }
            }

            throw new Exception($"Cannot unmarshal type '{type.Name}' with value '{strValue}'");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            if (value is null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStringValue(value.ToString());
        }
    }
}
