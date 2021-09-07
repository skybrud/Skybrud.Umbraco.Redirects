using Skybrud.Umbraco.Redirects.Models;
using System;
using System.Collections.Specialized;
using System.Web;

namespace Skybrud.Umbraco.Redirects.Events {
    
    /// <summary>
    /// Class representing the arguments of the <see cref="IRedirectsService.PostLookup"/> event, based on an <see cref="HttpContextBase"/>.
    /// </summary>
    public class RedirectPostLookupHttpContextEventArgs : RedirectPostLookupEventArgs {

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
        /// Initializes a new instance based on the specified HTTP <paramref name="context"/> and <paramref name="redirect"/>.
        /// </summary>
        /// <param name="context">The HTTP context of the request.</param>
        /// <param name="redirect">The redirect indicating where the user should be redirected to.</param>
        public RedirectPostLookupHttpContextEventArgs(HttpContextBase context, RedirectItem redirect) : base(redirect) {
            HttpContext = context;
            Redirect = redirect;
        }

        #endregion

    }

}