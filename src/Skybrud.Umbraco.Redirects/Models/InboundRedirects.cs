namespace Skybrud.Umbraco.Redirects.Models;

/// <summary>
/// Dummy class representing an inbound redirect. This class has no real purpose other than detecting properties
/// returning instances of this class.
/// </summary>
public class InboundRedirects {

    // Dummy model so that we can target it from ModelsBuilder, JSON converters etc.

    private static InboundRedirects? _instance;

    /// <summary>
    /// Gets a singleton instance of <see cref="InboundRedirects"/>.
    /// </summary>
    public static InboundRedirects Instance => _instance ??= new InboundRedirects();

}