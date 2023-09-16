using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GithubHelper.Models;

/// <summary>
/// A release.
/// </summary>
public partial class Release
{
    [JsonPropertyName("assets")]
    public List<ReleaseAsset> Assets { get; set; }

    [JsonPropertyName("assets_url")]
    public Uri AssetsUrl { get; set; }

    /// <summary>
    /// A GitHub user.
    /// </summary>
    [JsonPropertyName("author")]
    public AuthorClass Author { get; set; }

    [JsonPropertyName("body")]
    public string Body { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("body_html")]
    public string BodyHtml { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("body_text")]
    public string BodyText { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    /// <summary>
    /// The URL of the release discussion.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("discussion_url")]
    public Uri DiscussionUrl { get; set; }

    /// <summary>
    /// true to create a draft (unpublished) release, false to create a published one.
    /// </summary>
    [JsonPropertyName("draft")]
    public bool Draft { get; set; }

    [JsonPropertyName("html_url")]
    public Uri HtmlUrl { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("mentions_count")]
    public long? MentionsCount { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    /// <summary>
    /// Whether to identify the release as a prerelease or a full release.
    /// </summary>
    [JsonPropertyName("prerelease")]
    public bool Prerelease { get; set; }

    [JsonPropertyName("published_at")]
    public string PublishedAt { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("reactions")]
    public ReactionRollup Reactions { get; set; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }

    [JsonPropertyName("tarball_url")]
    public Uri TarballUrl { get; set; }

    /// <summary>
    /// Specifies the commitish value that determines where the Git tag is created from.
    /// </summary>
    [JsonPropertyName("target_commitish")]
    public string TargetCommitish { get; set; }

    [JsonPropertyName("upload_url")]
    public string UploadUrl { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }

    [JsonPropertyName("zipball_url")]
    public Uri ZipballUrl { get; set; }
}

/// <summary>
/// Data related to a release.
/// </summary>
public partial class ReleaseAsset
{
    [JsonPropertyName("browser_download_url")]
    public Uri BrowserDownloadUrl { get; set; }

    [JsonPropertyName("content_type")]
    public string ContentType { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }

    [JsonPropertyName("download_count")]
    public long DownloadCount { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    /// <summary>
    /// The file name of the asset.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    /// <summary>
    /// State of the release asset.
    /// </summary>
    [JsonPropertyName("state")]
    public State State { get; set; }

    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; }

    [JsonPropertyName("uploader")]
    public SimpleUser Uploader { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum State { Open, Uploaded };

/// <summary>
/// A GitHub user.
/// </summary>
public partial class SimpleUser
{
    [JsonPropertyName("avatar_url")]
    public Uri AvatarUrl { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; }

    [JsonPropertyName("followers_url")]
    public Uri FollowersUrl { get; set; }

    [JsonPropertyName("following_url")]
    public string FollowingUrl { get; set; }

    [JsonPropertyName("gists_url")]
    public string GistsUrl { get; set; }

    [JsonPropertyName("gravatar_id")]
    public string GravatarId { get; set; }

    [JsonPropertyName("html_url")]
    public Uri HtmlUrl { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    [JsonPropertyName("organizations_url")]
    public Uri OrganizationsUrl { get; set; }

    [JsonPropertyName("received_events_url")]
    public Uri ReceivedEventsUrl { get; set; }

    [JsonPropertyName("repos_url")]
    public Uri ReposUrl { get; set; }

    [JsonPropertyName("site_admin")]
    public bool SiteAdmin { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("starred_at")]
    public string StarredAt { get; set; }

    [JsonPropertyName("starred_url")]
    public string StarredUrl { get; set; }

    [JsonPropertyName("subscriptions_url")]
    public Uri SubscriptionsUrl { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }
}

/// <summary>
/// A GitHub user.
/// </summary>
public partial class AuthorClass
{
    [JsonPropertyName("avatar_url")]
    public Uri AvatarUrl { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("events_url")]
    public string EventsUrl { get; set; }

    [JsonPropertyName("followers_url")]
    public Uri FollowersUrl { get; set; }

    [JsonPropertyName("following_url")]
    public string FollowingUrl { get; set; }

    [JsonPropertyName("gists_url")]
    public string GistsUrl { get; set; }

    [JsonPropertyName("gravatar_id")]
    public string GravatarId { get; set; }

    [JsonPropertyName("html_url")]
    public Uri HtmlUrl { get; set; }

    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("login")]
    public string Login { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("node_id")]
    public string NodeId { get; set; }

    [JsonPropertyName("organizations_url")]
    public Uri OrganizationsUrl { get; set; }

    [JsonPropertyName("received_events_url")]
    public Uri ReceivedEventsUrl { get; set; }

    [JsonPropertyName("repos_url")]
    public Uri ReposUrl { get; set; }

    [JsonPropertyName("site_admin")]
    public bool SiteAdmin { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("starred_at")]
    public string StarredAt { get; set; }

    [JsonPropertyName("starred_url")]
    public string StarredUrl { get; set; }

    [JsonPropertyName("subscriptions_url")]
    public Uri SubscriptionsUrl { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }
}

public partial class ReactionRollup
{
    [JsonPropertyName("+1")]
    public long UpVotes { get; set; }

    [JsonPropertyName("-1")]
    public long DownVotes { get; set; }

    [JsonPropertyName("confused")]
    public long Confused { get; set; }

    [JsonPropertyName("eyes")]
    public long Eyes { get; set; }

    [JsonPropertyName("heart")]
    public long Heart { get; set; }

    [JsonPropertyName("hooray")]
    public long Hooray { get; set; }

    [JsonPropertyName("laugh")]
    public long Laugh { get; set; }

    [JsonPropertyName("rocket")]
    public long Rocket { get; set; }

    [JsonPropertyName("total_count")]
    public long TotalCount { get; set; }

    [JsonPropertyName("url")]
    public Uri Url { get; set; }
}