using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Notifications {
    
    /// <summary>
    /// Default implementation of the <see cref="IRedirectPostLookupNotification"/> interface, based on an <see cref="HttpContext"/>.
    /// </summary>
    public class RedirectPostLookupNotification : IRedirectPostLookupNotification {

        #region Properties

        /// <summary>
        /// Gets a reference to the <see cref="HttpContext"/> of the request.
        /// </summary>
        public HttpContext HttpContext { get; protected set; }

        /// <inheritdoc />
        public virtual Uri Uri => HttpContext.Request.GetUri();

        /// <inheritdoc />
        public virtual string RawUrl => HttpContext.Request.GetDisplayUrl();
        
        /// <summary>
        /// Gets the redirect.
        /// </summary>
        public IRedirect Redirect { get; }
        
        /// <summary>
        /// Gets or sets the type of the redirect. Only change this value if you wish to overwrite the default behaviour.
        /// </summary>
        public RedirectType RedirectType { get; set; }
        
        /// <summary>
        /// Gets or sets the destination URL of the redirect.
        /// </summary>
        public string DestinationUrl { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified HTTP <paramref name="context"/> and <paramref name="redirect"/>.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="redirect">The redirect indicating where the user should be redirected to.</param>
        /// <param name="redirectType">The redirect type.</param>
        /// <param name="destinationUrl">The destination URL.</param>
        public RedirectPostLookupNotification(HttpContext context, IRedirect redirect, RedirectType redirectType, string destinationUrl) {
            HttpContext = context;
            Redirect = redirect;
            RedirectType = redirectType;
            DestinationUrl = destinationUrl;
        }

        #endregion

    }

}