using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

#pragma warning disable CS1591

namespace Skybrud.Umbraco.Redirects.Models.Api {

    public class RedirectList {

        [JsonProperty("pagination")]
        [JsonPropertyName("pagination")]
        public RedirectsSearchResultPagination Pagination { get; }

        [JsonProperty("items")]
        [JsonPropertyName("items")]
        public IEnumerable<RedirectModel> Items { get; }

        public RedirectList(RedirectsSearchResultPagination pagination, IEnumerable<RedirectModel> items) {
            Pagination = pagination;
            Items = items;
        }

    }

}