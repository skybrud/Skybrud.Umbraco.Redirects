using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Notifications {
    
    public class RedirectPreLookupNotification : IRedirectPreLookupNotification {
        
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
        /// Gets or sets the redirect.
        /// </summary>
        public IRedirect Redirect { get; set; }
        
        /// <summary>
        /// Gets or sets the type of the redirect. Only change this value if you wish to overwrite the default behaviour.
        /// </summary>
        public RedirectType? RedirectType { get; set; }
        
        /// <summary>
        /// Gets or sets the destination URL of the redirect. Usually it should be necessary to change this value, but you can do so to force a specific destination URL.
        /// </summary>
        public string DestinationUrl { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified HTTP <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        public RedirectPreLookupNotification(HttpContext context) {
            HttpContext = context;
        }

        #endregion

    }

}