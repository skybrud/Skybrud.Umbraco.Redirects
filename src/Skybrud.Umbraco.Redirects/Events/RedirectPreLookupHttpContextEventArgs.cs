using System;
using System.Collections.Specialized;
using System.Web;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Events {
    
    /// <summary>
    /// Class representing the arguments of the <see cref="IRedirectsService.PreLookup"/> event, based on an <see cref="HttpContextBase"/>.
    /// </summary>
    public class RedirectPreLookupHttpContextEventArgs : RedirectPreLookupEventArgs {
        
        #region Properties

        /// <summary>
        /// Gets a reference to the <see cref="HttpContextBase"/> of the request.
        /// </summary>
        public HttpContextBase HttpContext { get; protected set; }

        /// <inheritdoc />
        public override Uri Uri => HttpContext.Request.Url;

        /// <inheritdoc />
        public override string RawUrl => HttpContext.Request.RawUrl;

        /// <inheritdoc />
        public override NameValueCollection QueryString => HttpContext.Request.QueryString;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified HTTP <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        public RedirectPreLookupHttpContextEventArgs(HttpContextBase context) {
            HttpContext = context;
        }

        /// <summary>
        /// Initializes a new instance based on the specified HTTP <paramref name="context"/> and <paramref name="redirect"/>.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="redirect">The redirect indicating where the user should be redirected to. If specified, further lookups will stop.</param>
        public RedirectPreLookupHttpContextEventArgs(HttpContextBase context, RedirectItem redirect) {
            HttpContext = context;
            Redirect = redirect;
        }

        #endregion

    }

}