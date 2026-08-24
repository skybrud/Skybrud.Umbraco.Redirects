using System.Collections.Generic;
using System.Text.Json.Serialization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Models.Api;

public class RedirectListModel {

    [JsonPropertyName("total")]
    public int Total { get; }

    [JsonPropertyName("items")]
    public IEnumerable<RedirectItem> Items { get; }

    public RedirectListModel(int total, IEnumerable<RedirectItem> items) {
        Total = total;
        Items = items;
    }

}