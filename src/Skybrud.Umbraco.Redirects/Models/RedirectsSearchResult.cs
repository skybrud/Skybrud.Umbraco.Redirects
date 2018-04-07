using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models {
    
    /// <summary>
    /// Class representing a collection of redirects.
    /// </summary>
    public class RedirectsSearchResult {

        #region Properties

        /// <summary>
        /// Gets pagination information about the collection.
        /// </summary>
        [JsonProperty("pagination")]
        public RedirectsSearchResultPagination Pagination { get; }

        /// <summary>
        /// Gets an array representing the items of the collection.
        /// </summary>
        [JsonProperty("items")]
        public RedirectItem[] Items { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified parameters.
        /// </summary>
        /// <param name="total">The total amount of redirects matched.</param>
        /// <param name="limit">The maximum amount of redirects to be returned per page.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="page">The page returned.</param>
        /// <param name="pages">The total amount of pages.</param>
        /// <param name="items">An array of the items making up the page.</param>
        public RedirectsSearchResult(int total, int limit, int offset, int page, int pages, RedirectItem[] items) {
            Pagination = new RedirectsSearchResultPagination(total, limit, offset, page, pages);
            Items = items;
        }

        #endregion

    }

}