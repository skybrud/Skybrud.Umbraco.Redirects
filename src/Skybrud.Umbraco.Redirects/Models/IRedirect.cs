using Newtonsoft.Json;
using Skybrud.Essentials.Time;
using System;
using System.Text.Json.Serialization;

namespace Skybrud.Umbraco.Redirects.Models;

/// <summary>
/// Interface describing a redirect.
/// </summary>
public interface IRedirect : IRedirectBase {

    /// <summary>
    /// Gets the ID of the redirect.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Gets the unique ID of the redirect.
    /// </summary>
    [JsonProperty("key")]
    [JsonPropertyName("key")]
    Guid Key { get; }

    /// <summary>
    /// Gets or sets the root node key of the redirect.
    /// </summary>
    [JsonProperty("rootKey")]
    [JsonPropertyName("rootKey")]
    Guid RootKey { get; set; }

    /// <summary>
    /// Gets or sets the inbound path of the redirect. The value will not contain the domain or the query string.
    /// </summary>
    [JsonProperty("path")]
    [JsonPropertyName("path")]
    string Path { get; set; }

    /// <summary>
    /// Gets or sets the inbound query string of the redirect.
    /// </summary>
    [JsonProperty("queryString")]
    [JsonPropertyName("queryString")]
    string? QueryString { get; set; }

    /// <summary>
    /// Gets or sets the inbound URL of the redirect.
    /// </summary>
    [JsonProperty("url")]
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>
    /// Gets or sets the destination of the redirect.
    /// </summary>
    public new IRedirectDestination Destination { get; set; }

    /// <summary>
    /// Gets or sets the type of the redirect. Possible values are <see cref="RedirectType.Permanent"/> and <see cref="RedirectType.Temporary"/>.
    /// </summary>
    public new RedirectType Type { get; set; }

    /// <summary>
    /// Gets or sets whether the redirect is permanent.
    /// </summary>
    public new bool IsPermanent { get; set; }

    /// <summary>
    /// Gets or sets whether the query string should be forwarded.
    /// </summary>
    public new bool ForwardQueryString { get; set; }

    /// <summary>
    /// Gets or sets the timestamp for when the redirect was created.
    /// </summary>
    public EssentialsTime CreateDate { get; }

    /// <summary>
    /// Gets or sets the timestamp for when the redirect was last updated.
    /// </summary>
    public EssentialsTime UpdateDate { get; set; }

}