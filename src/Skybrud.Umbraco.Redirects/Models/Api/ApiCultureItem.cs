namespace Skybrud.Umbraco.Redirects.Models.Api;

/// <summary>
/// Model class representing a culture item in the API.
/// </summary>
public class ApiCultureItem {

    /// <summary>
    /// Gets or sets the alias (or key) of the culture item. E.g. <c>da-DK</c> or <c>en-US</c>.
    /// </summary>
    public required string Alias { get; init; }

    /// <summary>
    /// Gets the name of the associated language, if available. E.g. <c>Danish (Denmark)</c> or <c>English (United States)</c>.
    /// </summary>
    public required string? Name { get; init; }

    /// <summary>
    /// Gets the culture-specific name of the node, if available.
    /// </summary>
    public required string? NodeName { get; init; }

    /// <summary>
    /// Gets the culture-specific URL of the node, if available.
    /// </summary>
    public required string? Url { get; init; }

}