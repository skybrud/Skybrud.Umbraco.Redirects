using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models {
    
    /// <summary>
    /// Class with pagination information about a collection of redirect items.
    /// </summary>
    public class RedirectsSearchResultPagination {

        #region Properties

        /// <summary>
        /// Gets the total amount of items across all pages.
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; }

        /// <summary>
        /// Gets the maximum amount of items per page.
        /// </summary>
        [JsonProperty("limit")]
        public int Limit { get; }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        [JsonProperty("offset")]
        public int Offset { get; }

        /// <summary>
        /// Gets the current page.
        /// </summary>
        [JsonProperty("page")]
        public int Page { get; }

        /// <summary>
        /// Gets the total amout of pages.
        /// </summary>
        [JsonProperty("pages")]
        public int Pages { get; }

        /// <summary>
        /// Gets the index of the first item on the page.
        /// </summary>
        [JsonProperty("from")]
        public int From { get; }

        /// <summary>
        /// Gets the index of the last item on the page.
        /// </summary>
        [JsonProperty("to")]
        public int To { get; }

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
        public RedirectsSearchResultPagination(int total, int limit, int offset, int page, int pages) {
            Total = total;
            Limit = limit;
            Offset = offset;
            Page = page;
            Pages = pages;
            From = offset + 1;
            To = Math.Min(offset + limit, total);
        }

        #endregion

    }

}