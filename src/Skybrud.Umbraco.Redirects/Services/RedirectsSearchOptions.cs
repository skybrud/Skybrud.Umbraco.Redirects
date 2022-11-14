using System;

namespace Skybrud.Umbraco.Redirects.Services {

    /// <summary>
    /// Class with options for searching through the created redirects.
    /// </summary>
    public class RedirectsSearchOptions {

        #region Properties

        /// <summary>
        /// Gets or sets the page to be returned. Default is <c>1</c>.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of redirects to be returned per page. Default is <c>20</c>.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Gets or sets the type of redirects to be returned. Possible values are
        /// <see cref="RedirectTypeFilter.All"/>, <see cref="RedirectTypeFilter.Content"/>,
        /// <see cref="RedirectTypeFilter.Media"/> or <see cref="RedirectTypeFilter.Url"/>. Default is
        /// <see cref="RedirectTypeFilter.All"/>.
        /// </summary>
        public RedirectTypeFilter Type { get; set; }

        /// <summary>
        /// Gets or sets a string value that should be present in either the text or URL of the returned redirects.
        /// Default is <c>null</c>.
        /// </summary>
        public string Text { get; set; }

		/// <summary>
		/// Gets or sets the key the returned redirects should match. <see cref="Guid.Empty"/> indicates all global
		/// redirects. Default is <c>null</c>, in which case this filter is disabled.
		/// </summary>
		[Obsolete("Obsoleted in favour of RootNodeKeys, as a user may have access to more than one root node.")]
        public Guid? RootNodeKey { get; set; }

		/// <summary>
		/// Gets or sets the key the returned redirects should match. <see cref="Guid.Empty"/> indicates all global
		/// redirects. Default is <c>null</c>, in which case this filter is disabled.
		/// </summary>
        public Guid[] RootNodeKeys { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance with default options.
        /// </summary>
        public RedirectsSearchOptions() {
            Page = 1;
            Limit = 20;
        }

        #endregion

    }

}