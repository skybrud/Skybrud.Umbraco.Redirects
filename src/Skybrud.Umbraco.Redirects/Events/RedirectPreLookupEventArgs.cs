using System;
using System.Collections.Specialized;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Events {
    
    /// <summary>
    /// Abstract class representing the arguments of the <see cref="IRedirectsService.PreLookup"/> event.
    /// </summary>
    public abstract class RedirectPreLookupEventArgs : EventArgs {

        #region Properties

        /// <summary>
        /// Gets the inbound URI of the request.
        /// </summary>
        public abstract Uri Uri { get; }

        /// <summary>
        /// Gets the raw URL of the request.
        /// </summary>
        public abstract string RawUrl { get; }

        /// <summary>
        /// Gets a reference to the <see cref="NameValueCollection"/> representing the query string of the request.
        /// </summary>
        public abstract NameValueCollection QueryString { get; }

        /// <summary>
        /// Gets or sets the redirect. If a redirect is set from the pre lookup event, the redirects package will use
        /// this redirect, and skip any further lookups.
        /// </summary>
        public RedirectItem Redirect { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance with default options.
        /// </summary>
        protected RedirectPreLookupEventArgs() { }

        /// <summary>
        /// Initializes a new instance based on the specified redirect.
        /// </summary>
        /// <param name="redirect">The redirect indicating where the user should be redirected to. If specified, further lookups will stop.</param>
        protected RedirectPreLookupEventArgs(RedirectItem redirect) {
            Redirect = redirect;
        }

        #endregion

    }

}