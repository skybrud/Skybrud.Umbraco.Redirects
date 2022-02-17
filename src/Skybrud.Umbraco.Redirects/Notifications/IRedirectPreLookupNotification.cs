using System;
using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Cms.Core.Notifications;

namespace Skybrud.Umbraco.Redirects.Notifications {
    
    /// <summary>
    /// Interface describing the redirect pre lookup noticication.
    /// </summary>
    public interface IRedirectPreLookupNotification : INotification {

        /// <summary>
        /// Gets the inbound URI of the request.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// Gets the raw URL of the request.
        /// </summary>
        string RawUrl { get; }

        /// <summary>
        /// Gets or sets the redirect. If a redirect is set from the pre lookup notification, the redirects package will use
        /// this redirect, and skip any further lookups.
        /// </summary>
        IRedirect Redirect { get; set; }

        /// <summary>
        /// Gets or sets the type of the redirect. Only change this value if you wish to overwrite the default behaviour.
        /// </summary>
        RedirectType? RedirectType { get; set; }

        /// <summary>
        /// Gets or sets the destination URL of the redirect. Usually it should be necessary to change this value, but you can do so to force a specific destination URL.
        /// </summary>
        string DestinationUrl { get; set; }

    }

}