using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.wwwroot.Modals.Outbound;

/// <summary>
/// Interface describing an outbound redirect.
/// </summary>
public interface IOutboundRedirect : IRedirectBase {

    #region Properties

    /// <summary>
    /// Same as <see cref="IsValid"/>.
    /// </summary>
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public bool HasDestination => IsValid;

    /// <summary>
    /// Gets whether the redirect has a valid link.
    /// </summary>
    [JsonIgnore]
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsValid => !string.IsNullOrWhiteSpace(Destination.Url);

    #endregion

}