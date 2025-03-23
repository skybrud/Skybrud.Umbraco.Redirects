using System;

namespace Skybrud.Umbraco.Redirects.Models;

/// <summary>
/// Interface describing the destination of a redirect.
/// </summary>
public interface IRedirectDestination {

    /// <summary>
    /// Gets the ID of the selected content or media. If a URL has been selected, this will return <c>0</c>.
    /// </summary>
    int Id { get; set; }

    /// <summary>
    /// Gets the GUID key of the selected content or media. If a URL has been selected, this will return <c>null</c>.
    /// </summary>
    Guid Key { get; set; }

    /// <summary>
    /// Gets the name of the destination.
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// Gets the URL of the destination.
    /// </summary>
    string Url { get; set; }

    /// <summary>
    /// Gets the query string part of the destination.
    /// </summary>
    string? Query { get; set; }

    /// <summary>
    /// Gets the fragment of the destination - e.g. <c>#hello</c>.
    /// </summary>
    string? Fragment { get; set; }

    /// <summary>
    /// Gets the full destination URL.
    /// </summary>
    string FullUrl { get; }

    /// <summary>
    /// Gets the type of the destination.
    /// </summary>
    RedirectDestinationType Type { get; set; }

    /// <summary>
    /// Gets the culture of the destination, if any.
    /// </summary>
    public string? Culture => null;

}