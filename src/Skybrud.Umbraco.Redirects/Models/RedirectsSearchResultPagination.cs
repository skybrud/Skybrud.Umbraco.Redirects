using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models {
    
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

        [JsonProperty("from")]
        public int From { get; }

        [JsonProperty("to")]
        public int To { get; }

        #endregion

        #region Constructors

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