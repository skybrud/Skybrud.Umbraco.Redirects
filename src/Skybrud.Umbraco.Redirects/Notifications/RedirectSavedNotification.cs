using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Cms.Core.Notifications;

namespace Skybrud.Umbraco.Redirects.Notifications;

/// <summary>
/// Notification raised when a redirect has been saved. This notification is raised after the redirect has been saved in the database, and can be used for performing custom actions after a redirect has been saved - e.g. clearing a custom cache of redirects.
/// </summary>
/// <remarks>This notification is only raised when an existing redirect is saved (aka updated), not when the redirect is initially added.</remarks>
public class RedirectSavedNotification : INotification {

    /// <summary>
    /// Gets the redirect that has been saved.
    /// </summary>
    public IRedirect Redirect { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="redirect"/> that has been saved.
    /// </summary>
    /// <param name="redirect">The redirect.</param>
    public RedirectSavedNotification(IRedirect redirect) {
        Redirect = redirect;
    }

}