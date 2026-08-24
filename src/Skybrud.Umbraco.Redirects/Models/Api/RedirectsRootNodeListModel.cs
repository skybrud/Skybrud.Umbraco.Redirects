using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Models.Api;

/// <summary>
/// Class representing a list of root nodes in the API.
/// </summary>
public class RedirectsRootNodeListModel {

    /// <summary>
    /// Gets the total amount of items in the list.
    /// </summary>
    public int Total { get; }

    /// <summary>
    /// Gets the items making up the list.
    /// </summary>
    public IReadOnlyList<RedirectsRootNodeModel> Items { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="items"/>.
    /// </summary>
    /// <param name="items">The items making up the list.</param>
    public RedirectsRootNodeListModel(IReadOnlyList<RedirectsRootNodeModel> items) {
        Total = items.Count;
        Items = items;
    }

}