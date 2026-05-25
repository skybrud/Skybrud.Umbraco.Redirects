using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Cms.Core.Notifications;

namespace Skybrud.Umbraco.Redirects.Notifications;

/// <summary>
/// Notification raised when a redirect has been added. This notification is raised after the redirect has been added in the database, and can be used for performing custom actions after a redirect has been added - e.g. clearing a custom cache of redirects.
/// </summary>
public class RedirectAddedNotification : INotification {

    /// <summary>
    /// Gets the redirect that has been added.
    /// </summary>
    public IRedirect Redirect { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="redirect"/> that has been added.
    /// </summary>
    /// <param name="redirect">The redirect.</param>
    public RedirectAddedNotification(IRedirect redirect) {
        Redirect = redirect;
    }

}