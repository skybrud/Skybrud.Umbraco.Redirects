using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Skybrud.Essentials.Strings.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Models;

/// <summary>
/// Class with information about the destination of a redirect.
/// </summary>
public class RedirectDestination : IRedirectDestination {

    #region Properties

    /// <summary>
    /// Gets the ID of the selected content or media. If a URL has been selected, this will return <c>0</c>.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets the GUID key of the selected content or media. If a URL has been selected, this will return <see cref="Guid.Empty"/>.
    /// </summary>
    public required Guid Key { get; set; }

    /// <summary>
    /// Gets the name of the destination, if any.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the URL of the destination.
    /// </summary>
    public required string Url { get; set; }

    /// <summary>
    /// Gets the query string part of the destination - e.g. <c>?hello=there</c>.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Query { get; set; }

    /// <summary>
    /// Gets the fragment of the destination - e.g. <c>#hello</c>.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Fragment { get; set; }

    /// <summary>
    /// Gets the full destination URL.
    /// </summary>
    public string FullUrl {
        get {
            StringBuilder sb = new();
            sb.Append(Url);
            if (Query.HasValue()) {
                sb.Append(Url.Contains('?') ? '&' : '?');
                sb.Append(Query);
            }
            if (Fragment.HasValue()) {
                sb.Append(Fragment);
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Gets the type of the destination.
    /// </summary>
    public required RedirectDestinationType Type { get; set; }

    /// <summary>
    /// Gets or sets the culture of the destination, if any.
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Culture { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance with default options.
    /// </summary>
    public RedirectDestination() { }

    /// <summary>
    ///
    /// </summary>
    /// <param name="content"></param>
    /// <exception cref="Exception"></exception>
    [SetsRequiredMembers]
    public RedirectDestination(IPublishedContent content) {
        Id = content.Id;
        Key = content.Key;
        Name = content.Name;
        Url = content.Url();
        Query = null;
        Fragment = null;
        Type = content.ItemType switch {
            PublishedItemType.Content => RedirectDestinationType.Content,
            PublishedItemType.Media => RedirectDestinationType.Media,
            _ => throw new Exception($"Unsupported item type: {content.ItemType}...")
        };
        Culture = null;
    }

    /// <summary>
    /// Initialize a new content or media destination based on the specified <paramref name="content"/>.
    /// </summary>
    /// <param name="content">The content or media item.</param>
    /// <param name="query">The query string, or <see langword="null"/>.</param>
    /// <param name="fragment">The fragment, or <see langword="null"/>.</param>
    /// <param name="culture">The culture, or <see langword="null"/>.</param>
    [SetsRequiredMembers]
    public RedirectDestination(IPublishedContent content, string? query, string? fragment, string? culture) {
        Id = content.Id;
        Key = content.Key;
        Name = content.Name;
        Url = content.Url();
        Query = query;
        Fragment = fragment;
        Type = content.ItemType switch {
            PublishedItemType.Content => RedirectDestinationType.Content,
            PublishedItemType.Media => RedirectDestinationType.Media,
            _ => throw new Exception($"Unsupported item type: {content.ItemType}...")
        };
        Culture = culture;
    }

    #endregion

}