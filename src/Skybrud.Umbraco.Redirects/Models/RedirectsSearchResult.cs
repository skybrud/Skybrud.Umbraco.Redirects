using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models {
    
    public class RedirectsSearchResult {

        #region Properties

        [JsonProperty("pagination")]
        public RedirectsSearchResultPagination Pagination { get; private set; }

        [JsonProperty("items")]
        public RedirectItem[] Items { get; private set; }

        #endregion

        #region Constructors

        public RedirectsSearchResult(int total, int limit, int offset, int page, int pages, RedirectItem[] items) {
            Pagination = new RedirectsSearchResultPagination(total, limit, offset, page, pages);
            Items = items;
        }

        #endregion

    }

}