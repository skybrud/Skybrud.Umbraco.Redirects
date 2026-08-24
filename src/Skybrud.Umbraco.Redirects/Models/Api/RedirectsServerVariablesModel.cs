using Skybrud.Umbraco.Redirects.Models.Settings;

namespace Skybrud.Umbraco.Redirects.Models.Api;

/// <summary>
/// Represents the server variables returned by the API.
/// </summary>
public class RedirectsServerVariablesModel {

    /// <summary>
    /// Gets or sets the version of the Redirects package
    /// </summary>
    public required string Version { get; init; }

    /// <summary>
    /// Gets or sets the cache buster value used for cache busting back-office assets within the Redirects package.
    /// </summary>
    public required string CacheBuster { get; init; }

    /// <summary>
    /// Gets or sets the settings of the Redirects package.
    /// </summary>
    public required RedirectsSettings Settings { get; init; }

}