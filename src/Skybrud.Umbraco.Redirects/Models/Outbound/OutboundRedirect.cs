using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.wwwroot.Modals.Outbound;

namespace Skybrud.Umbraco.Redirects.Models.Outbound;

/// <summary>
/// Model for an outbound redirect.
/// </summary>
public class OutboundRedirect : IOutboundRedirect {

    #region Properties

    /// <summary>
    /// Gets whether the redirect is permanent.
    /// </summary>
    [JsonProperty("permanent")]
    [JsonPropertyName("permanent")]
    public bool IsPermanent => Type == RedirectType.Permanent;

    /// <summary>
    /// Gets the type of the redirect - either <see cref="RedirectType.Permanent"/> or <see cref="RedirectType.Temporary"/>.
    /// </summary>
    [JsonProperty("type")]
    [JsonPropertyName("type")]
    public required RedirectType Type { get; set; }

    /// <summary>
    /// Gets an instance of <see cref="RedirectDestination"/> representing the destination.
    /// </summary>
    [JsonProperty("destination")]
    [JsonPropertyName("destination")]
    public required IRedirectDestination Destination { get; set; }

    /// <summary>
    /// Gets whether the query string of the inbound request should be forwarded.
    /// </summary>
    [JsonProperty("forward")]
    [JsonPropertyName("forward")]
    public bool ForwardQueryString { get; set; }

    /// <summary>
    /// Same as <see cref="IsValid"/>.
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public bool HasDestination => IsValid;

    /// <summary>
    /// Gets whether the redirect has a valid link.
    /// </summary>
    [Newtonsoft.Json.JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsValid => !string.IsNullOrWhiteSpace(Destination.Url);

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of <see cref="OutboundRedirect"/> with default values.
    /// </summary>
    public OutboundRedirect() { }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="type"/>, <paramref name="forward"/> and <paramref name="destination"/>.
    /// </summary>
    /// <param name="type">The type of the redirect.</param>
    /// <param name="forward">Whether query string forwarding should be enabled.</param>
    /// <param name="destination">The destination of the redirect.</param>
    [SetsRequiredMembers]
    public OutboundRedirect(RedirectType type, bool forward, IRedirectDestination destination) {
        Type = type;
        ForwardQueryString = forward;
        Destination = destination;
    }

    #endregion

}