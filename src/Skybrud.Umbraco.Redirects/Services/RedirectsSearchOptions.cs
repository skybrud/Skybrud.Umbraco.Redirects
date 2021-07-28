using System;

namespace Skybrud.Umbraco.Redirects.Services {
    
    public class RedirectsSearchOptions {

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
        /// <see RedirectTypetsType.All"/>, <see RedirectTypetsType.Content"/>,
        /// <see RedirectTypetsType.Media"/> or <see RedirectTypetsType.Url"/>. Default is
        /// <see RedirectTypetsType.All"/>.
        /// </summary>
        public RedirectType Type { get; set; }

        /// <summary>
        /// Gets or sets a string value that should be present in either the text or URL of the returned redirects.
        /// Default is <c>null</c>.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the key the returned redirects should match. <see cref="Guid.Empty"/> indicates all global
        /// redirects. Default is <c>null</c>, in which case this filter is disabled.
        /// </summary>
        public Guid? RootNodeKey { get; set; }

        public RedirectsSearchOptions() {
            Page = 1;
            Limit = 20;
        }

    }

}