using System.Text.Json.Serialization;

namespace MambaManager;

public partial class MambaResponse
{
    [JsonPropertyName("envs")]
    public List<string> Envs { get; set; }

    [JsonPropertyName("solver_problems")]
    public List<string> Problems { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("actions")]
    public Actions Actions { get; set; }

    [JsonPropertyName("dry_run")]
    public bool DryRun { get; set; }

    [JsonPropertyName("prefix")]
    public string Prefix { get; set; }
}

public partial class Actions
{
    [JsonPropertyName("FETCH")]
    public List<Fetch> Fetch { get; set; }

    [JsonPropertyName("LINK")]
    public List<Fetch> Link { get; set; }

    [JsonPropertyName("PREFIX")]
    public string Prefix { get; set; }
}

public partial class Fetch
{
    [JsonPropertyName("build")]
    public string Build { get; set; }

    [JsonPropertyName("build_number")]
    public long BuildNumber { get; set; }

    [JsonPropertyName("build_string")]
    public string BuildString { get; set; }

    [JsonPropertyName("channel")]
    public Uri Channel { get; set; }

    [JsonPropertyName("constrains")]
    public List<string> Constrains { get; set; }

    [JsonPropertyName("depends")]
    public List<string> Depends { get; set; }

    [JsonPropertyName("fn")]
    public string Fn { get; set; }

    [JsonPropertyName("license")]
    public string License { get; set; }

    [JsonPropertyName("md5")]
    public string Md5 { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("subdir")]
    public string Subdir { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("track_features")]
    public string TrackFeatures { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }
}