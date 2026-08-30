using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Services;

/// <summary>
/// Interface describing the redirects service.
/// </summary>
public interface IRedirectsService {

    #region Redirect CRUD methods

    /// <summary>
    /// Adds a new redirect with the specified <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options describing the redirect.</param>
    /// <returns>An instance of <see cref="IRedirect"/> representing the created redirect.</returns>
    IRedirect AddRedirect(AddRedirectOptions options);

    /// <summary>
    /// Returns the redirect with the specified numeric <paramref name="id"/>, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="id">The numeric ID of the redirect.</param>
    /// <returns>An instance of <see cref="IRedirect"/>, or <see langword="null"/> if not found.</returns>
    IRedirect? GetRedirectById(int id);

    /// <summary>
    /// Returns the redirect mathing the specified GUID <paramref name="key"/>, or <see langword="null"/> if not found.
    /// </summary>
    /// <param name="key">The GUID key of the redirect.</param>
    /// <returns>An instance of <see cref="IRedirect"/>, or <see langword="null"/> if not found.</returns>
    IRedirect? GetRedirectByKey(Guid key);

    /// <summary>
    /// Returns the redirect matching the specified <paramref name="path"/> and <paramref name="query"/>, or <see langword="null"/> if not redirect is found.
    /// </summary>
    /// <param name="rootNodeKey">The GUID of the root/side node. Use <see cref="Guid.Empty"/> for a global redirect.</param>
    /// <param name="path">The path of the inbound request.</param>
    /// <param name="query">The query string of the inbound request.</param>
    /// <returns>An instance of <see cref="IRedirect"/>, or <see langword="null"/> if no matching redirect is found.</returns>
    IRedirect? GetRedirectByPathAndQuery(Guid rootNodeKey, string path, string? query);

    /// <summary>
    /// Returns the first redirect matching the specified <paramref name="request"/>, or <see langword="null"/> if the request does not match any redirects.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>An instance of <see cref="IRedirect"/>, or <see langword="null"/> if no matching redirects were found.</returns>
    IRedirect? GetRedirectByRequest(HttpRequest request);

    /// <summary>
    /// Returns the first redirect matching the specified <paramref name="uri"/>, or <see langword="null"/> if the URI does not match any redirects.
    /// </summary>
    /// <param name="uri">The URI of the request.</param>
    /// <returns>An instance of <see cref="IRedirect"/>, or <see langword="null"/> if no matching redirects were found.</returns>
    IRedirect? GetRedirectByUri(Uri uri);

    /// <summary>
    /// Gets the redirect matching the specified <paramref name="url"/>.
    /// </summary>
    /// <param name="rootNodeKey">The GUID of the root/side node. Use <see cref="Guid.Empty"/> for a global redirect.</param>
    /// <param name="url">The URL of the redirect.</param>
    /// <returns>An instance of <see cref="IRedirect"/>, or <see langword="null"/> if not found.</returns>
    IRedirect? GetRedirectByUrl(Guid rootNodeKey, string url);

    /// <summary>
    /// Returns a redirect matching the specified <paramref name="url"/>, optionally limited to the specified root node.
    /// </summary>
    /// <param name="url">The URL of the redirect.</param>
    /// <param name="rootNodeKey"> The key of the root node to match, or <see langword="null"/> to match redirects across all root nodes.</param>
    /// <returns>An instance of <see cref="IRedirect"/>, or <see langword="null"/> if no matching redirect is found.</returns>
    IRedirect? GetRedirectByUrl(string url, Guid? rootNodeKey = null);

    /// <summary>
    /// Returns redirects matching the specified <paramref name="url"/>, optionally limited to the specified root node.
    /// </summary>
    /// <param name="url">The URL of the redirects.</param>
    /// <param name="rootNodeKey"> The key of the root node to match, or <see langword="null"/> to match redirects across all root nodes.</param>
    /// <returns>A list of matching redirects.</returns>
    IReadOnlyList<IRedirect> GetRedirectsByUrl(string url, Guid? rootNodeKey = null);

    /// <summary>
    /// Returns a list of all redirects.
    /// </summary>
    /// <returns>A list of <see cref="IRedirect"/>.</returns>
    IReadOnlyList<IRedirect> GetRedirects();

    /// <summary>
    /// Returns a paginated list of redirects matching the specified <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options the returned redirects should match.</param>
    /// <returns>An instance of <see cref="RedirectsSearchResult"/>.</returns>
    RedirectsSearchResult GetRedirects(RedirectsSearchOptions options);

    /// <summary>
    /// Returns an array of redirects where the destination matches the specified <paramref name="nodeType"/> and <paramref name="nodeId"/>.
    /// </summary>
    /// <param name="nodeType">The type of the destination node.</param>
    /// <param name="nodeId">The numeric ID of the destination node.</param>
    /// <returns>A list of <see cref="IRedirect"/>.</returns>
    IReadOnlyList<IRedirect> GetRedirectsByNodeId(RedirectDestinationType nodeType, int nodeId);

    /// <summary>
    /// Returns an array of redirects where the destination matches the specified <paramref name="nodeType"/>, <paramref name="nodeId"/> and <paramref name="culture"/>.
    /// </summary>
    /// <param name="nodeType">The type of the destination node.</param>
    /// <param name="nodeId">The numeric ID of the destination node.</param>
    /// <param name="culture">The culture the returned redirects should match.</param>
    /// <returns>A list of <see cref="IRedirect"/>.</returns>
    IReadOnlyList<IRedirect> GetRedirectsByNodeId(RedirectDestinationType nodeType, int nodeId, string? culture);

    /// <summary>
    /// Returns an array of redirects where the destination matches the specified <paramref name="nodeType"/> and <paramref name="nodeKey"/>.
    /// </summary>
    /// <param name="nodeType">The type of the destination node.</param>
    /// <param name="nodeKey">The key (GUID) of the destination node.</param>
    /// <returns>A list of <see cref="IRedirect"/>.</returns>
    IReadOnlyList<IRedirect> GetRedirectsByNodeKey(RedirectDestinationType nodeType, Guid nodeKey);

    /// <summary>
    /// Returns a list of redirects where the destination matches the specified <paramref name="nodeType"/>, <paramref name="nodeKey"/> and <paramref name="culture"/>.
    /// </summary>
    /// <param name="nodeType">The type of the destination node.</param>
    /// <param name="nodeKey">The key (GUID) of the destination node.</param>
    /// <param name="culture">The culture the returned redirects should match.</param>
    /// <returns>A list of <see cref="IRedirect"/>.</returns>
    IReadOnlyList<IRedirect> GetRedirectsByNodeKey(RedirectDestinationType nodeType, Guid nodeKey, string? culture);

    /// <summary>
    /// Saves the specified <paramref name="redirect"/>.
    /// </summary>
    /// <param name="redirect">The redirecte to be saved.</param>
    /// <returns>The saved <paramref name="redirect"/>.</returns>
    IRedirect SaveRedirect(IRedirect redirect);

    /// <summary>
    /// Deletes the specified <paramref name="redirect"/>.
    /// </summary>
    /// <param name="redirect">The redirect to be deleted.</param>
    void DeleteRedirect(IRedirect redirect);

    #endregion

    #region Redirect destination URL

    /// <summary>
    /// Returns the calculated destination URL for the specified <paramref name="redirect"/>.
    /// </summary>
    /// <param name="redirect">The redirect.</param>
    /// <returns>The destination URL.</returns>
    string GetDestinationUrl(IRedirectBase redirect) {
        return GetDestinationUrl(redirect, null);
    }

    /// <summary>
    /// Returns the calculated destination URL for the specified <paramref name="redirect"/>.
    /// </summary>
    /// <param name="redirect">The redirect.</param>
    /// <param name="uri">The inbound URL.</param>
    /// <returns>The destination URL.</returns>
    string GetDestinationUrl(IRedirectBase redirect, Uri? uri);

    #endregion

    #region Other

    /// <summary>
    /// Returns a list of all domains (<see cref="RedirectDomain"/>) registered in Umbraco.
    /// </summary>
    /// <returns>A list of <see cref="RedirectDomain"/>.</returns>
    IReadOnlyList<RedirectDomain> GetDomains();

    /// <summary>
    /// Returns a list of all rode nodes configured in Umbraco.
    /// </summary>
    /// <returns>A list of <see cref="RedirectRootNode"/> representing the root nodes.</returns>
    IReadOnlyList<RedirectRootNode> GetRootNodes();

    #endregion

}