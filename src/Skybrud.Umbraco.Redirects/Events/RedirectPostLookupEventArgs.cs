using Skybrud.Umbraco.Redirects.Models;
using System;
using System.Collections.Specialized;

namespace Skybrud.Umbraco.Redirects.Events {
    
    /// <summary>
    /// Abstract class representing the arguments of the <see cref="IRedirectsService.PostLookup"/> event.
    /// </summary>
    public abstract class RedirectPostLookupEventArgs : EventArgs {

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
        /// Gets or sets the redirect.
        /// </summary>
        public RedirectItem Redirect { get; set; }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initializes a new instance based on the specified redirect.
        /// </summary>
        /// <param name="redirect">The redirect indicating where the user should be redirected to.</param>
        protected RedirectPostLookupEventArgs(RedirectItem redirect) {
            Redirect = redirect;
        }

        #endregion

    }

}