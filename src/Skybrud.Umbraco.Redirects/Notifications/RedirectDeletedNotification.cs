using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Cms.Core.Notifications;

namespace Skybrud.Umbraco.Redirects.Notifications;

/// <summary>
/// Notification raised when a redirect has been deleted. This notification is raised after the redirect has been deleted from the database, and can be used for performing custom actions after a redirect has been deleted - e.g. clearing a custom cache of redirects.
/// </summary>
public class RedirectDeletedNotification : INotification {

    /// <summary>
    /// Gets the redirect that has been deleted.
    /// </summary>
    public IRedirect Redirect { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="redirect"/> that has been deleted.
    /// </summary>
    /// <param name="redirect">The redirect.</param>
    public RedirectDeletedNotification(IRedirect redirect) {
        Redirect = redirect;
    }

}