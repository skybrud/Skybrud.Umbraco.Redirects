using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Skybrud.Umbraco.Redirects.Models.Api;

#pragma warning disable CS1591

public class RedirectSearchResultModel {

    [JsonPropertyName("pagination")]
    public RedirectListPagination Pagination { get; }

    [JsonPropertyName("items")]
    public IEnumerable<RedirectItem> Items { get; }

    public RedirectSearchResultModel(RedirectListPagination pagination, IEnumerable<RedirectItem> items) {
        Pagination = pagination;
        Items = items;
    }

}