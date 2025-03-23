namespace Skybrud.Umbraco.Redirects.Models;

/// <summary>
/// Interface describing a basic redirect.
/// </summary>
/// <seealso cref="IRedirect"/>
/// <seealso cref="Redirect"/>
// /// <seealso cref="IOutboundRedirect"/>
// /// <seealso cref="OutboundRedirect"/>
public interface IRedirectBase {

    /// <summary>
    /// Gets or sets whether the redirect is permanent.
    /// </summary>
    public bool IsPermanent => Type == RedirectType.Permanent;

    /// <summary>
    /// Gets or sets the type of the redirect. Possible values are <see cref="RedirectType.Permanent"/> and <see cref="RedirectType.Temporary"/>.
    /// </summary>
    public RedirectType Type { get; set; }

    /// <summary>
    /// Gets or sets whether the query string should be forwarded.
    /// </summary>
    public bool ForwardQueryString { get; set; }

    /// <summary>
    /// Gets or sets the destination of the redirect.
    /// </summary>
    public IRedirectDestination Destination { get; set; }

}