using System;
using System.Net;

namespace Skybrud.Umbraco.Redirects.Exceptions;

/// <summary>
/// Exception about a redirect that couldn't be found.
/// </summary>
public class RedirectNotFoundException : RedirectsException {

    /// <summary>
    /// Gets the ID of the redirect, or <see langword="null"/> if the redirect wasn't requested by its numeric ID.
    /// </summary>
    public int? Id { get; }

    /// <summary>
    /// Gets the key of the redirect, or <see langword="null"/> if the redirect wasn't requested by its GUID key.
    /// </summary>
    public Guid? Key { get; }

    /// <summary>
    /// Gets the URL of the redirect, or <see langword="null"/> if the redirect wasn't requested by its URL.
    /// </summary>
    public string? Url { get; }

    /// <summary>
    /// Gets the key of the root node used when looking up the redirect by URL, or <see langword="null"/> if no root node was specified.
    /// </summary>
    public Guid? RootNodeKey { get; }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="id"/>.
    /// </summary>
    /// <param name="id">The ID of the redirect.</param>
    public RedirectNotFoundException(int id) : base(HttpStatusCode.NotFound, "A redirect with the specified ID could not be found.") {
        Id = id;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="key"/>.
    /// </summary>
    /// <param name="key">The GUID key of the redirect.</param>
    public RedirectNotFoundException(Guid key) : base(HttpStatusCode.NotFound, "A redirect with the specified key could not be found.") {
        Key = key;
    }

    /// <summary>
    /// Initializes a new instance based on the specified <paramref name="url"/>.
    /// </summary>
    /// <param name="url">The URL of the redirect.</param>
    /// <param name="rootNodeKey">The key of the root node used, or <see langword="null"/> if no root node was specified.</param>
    public RedirectNotFoundException(string url, Guid? rootNodeKey) : base(HttpStatusCode.NotFound, "A redirect with the specified URL could not be found.") {
        Url = url;
        RootNodeKey = rootNodeKey;
    }

    ///// <summary>
    ///// Initializes a new exception with the specified <paramref name="message"/>.
    ///// </summary>
    ///// <param name="message">The error message.</param>
    //public RedirectNotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }

}