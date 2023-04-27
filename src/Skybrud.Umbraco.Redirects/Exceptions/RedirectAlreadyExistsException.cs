using System;
using System.Globalization;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Cms.Core.Services;

namespace Skybrud.Umbraco.Redirects.Exceptions {

    /// <summary>
    /// Class representing an exception thrown when the user tries to create a redirect for an URL that is already used by another redirect.
    /// </summary>
    public class RedirectAlreadyExistsException : RedirectsLocalizedException {

        #region Properties

        /// <summary>
        /// Gets the GUID key of the selected root node. Is <see cref="Guid.Empty"/> if not root node has been selected.
        /// </summary>
        public Guid RootNodeKey { get; }

        /// <summary>
        /// Gets the path of the inbound URL.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the query string of the inbound URL.
        /// </summary>
        public string? QueryString { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        public RedirectAlreadyExistsException(IRedirect redirect) : this(redirect.RootKey, redirect.Path, redirect.QueryString) { }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="rootNodeKey"/>, <paramref name="path"/> and <paramref name="queryString"/>.
        /// </summary>
        /// <param name="rootNodeKey">The GUID key of the root node.</param>
        /// <param name="path">The path of the inbound URL.</param>
        /// <param name="queryString">The query string of the inbound URL.</param>
        public RedirectAlreadyExistsException(Guid rootNodeKey, string path, string? queryString) : base("A redirect with the same URL and query string already exists.") {
            RootNodeKey = rootNodeKey;
            Path = path;
            QueryString = queryString.NullIfWhiteSpace();
        }

        #endregion

        #region Member methods

        internal override string GetLocalizedMessage(ILocalizedTextService textService, CultureInfo? culture)  {
            return textService.Localize("redirects", "errorRedirectAlreadyExists", culture);
        }

        #endregion

    }

}