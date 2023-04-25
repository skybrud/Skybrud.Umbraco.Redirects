using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Http;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Options;

using Umbraco.Cms.Core.Models.Membership;

namespace Skybrud.Umbraco.Redirects.Services {

    /// <summary>
    /// Interface describing the redirects service.
    /// </summary>
    public interface IRedirectsService {

        /// <summary>
        /// Returns an array of <see cref="RedirectDomain"/> representing all the domains registered in Umbraco.
        /// </summary>
        /// <returns>An array of <see cref="RedirectDomain"/>.</returns>
        RedirectDomain[] GetDomains();

        /// <summary>
        /// Adds a new redirect with the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options describing the redirect.</param>
        /// <returns>An instance of <see cref="IRedirect"/> representing the created redirect.</returns>
        IRedirect AddRedirect(AddRedirectOptions options);

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

        /// <summary>
        /// Returns the redirect with the  specified numeric <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="redirectId">The numeric ID of the redirect.</param>
        /// <returns>An instance of <see cref="IRedirect"/>, or <c>null</c> if not found.</returns>
        IRedirect? GetRedirectById(int redirectId);

        /// <summary>
        /// Gets the redirect mathing the specified GUID <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The GUID key of the redirect.</param>
        /// <returns>An instance of <see cref="IRedirect"/>, or <c>null</c> if not found.</returns>
        IRedirect? GetRedirectByKey(Guid key);

        /// <summary>
        /// Gets the redirect matching the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="rootNodeKey">The GUID of the root/side node. Use <see cref="Guid.Empty"/> for a global redirect.</param>
        /// <param name="url">The URL of the redirect.</param>
        /// <returns>An instance of <see cref="IRedirect"/>, or <c>null</c> if not found.</returns>
        IRedirect? GetRedirectByUrl(Guid rootNodeKey, string url);

        /// <summary>
        /// Returns the redirect matching the specified <paramref name="path"/> and <paramref name="query"/>, or <c>null</c> if not redirect is found.
        /// </summary>
        /// <param name="rootNodeKey">The GUID of the root/side node. Use <see cref="Guid.Empty"/> for a global redirect.</param>
        /// <param name="path">The path of the inbound inbound request.</param>
        /// <param name="query">The query string of the inbound request.</param>
        /// <returns>An instance of <see cref="IRedirect"/>, or <c>null</c> if no matching redirect found.</returns>
        IRedirect? GetRedirectByPathAndQuery(Guid rootNodeKey, string path, string? query);

        /// <summary>
        /// Returns the first redirect matching the specified <paramref name="request"/>, or <c>null</c> if the request does not match any redirects.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An instance of <see cref="IRedirect"/>, or <c>null</c> if no matching redirects were found.</returns>
        IRedirect? GetRedirectByRequest(HttpRequest request);

        /// <summary>
        /// Returns the first redirect matching the specified <paramref name="uri"/>, or <c>null</c> if the URI does not match any redirects.
        /// </summary>
        /// <param name="uri">The URI of the request.</param>
        /// <returns>An instance of <see cref="IRedirect"/>, or <c>null</c> if no matching redirects were found.</returns>
        IRedirect? GetRedirectByUri(Uri uri);

        /// <summary>
        /// Returns the calculated destination URL for the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <returns>The destination URL.</returns>
        string GetDestinationUrl(IRedirectBase redirect);

        /// <summary>
        /// Returns the calculated destination URL for the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <param name="uri">The inbound URL.</param>
        /// <returns>The destination URL.</returns>
        string GetDestinationUrl(IRedirectBase redirect, Uri uri);

        /// <summary>
        /// Returns a paginated list of redirects matching the specified <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The options the returned redirects should match.</param>
        /// <returns>An instance of <see cref="RedirectsSearchResult"/>.</returns>
        RedirectsSearchResult GetRedirects(RedirectsSearchOptions options);

        /// <summary>
        /// Returns a collection with all redirects.
        /// </summary>
        /// <returns>An instance of <see cref="IEnumerable{Redirect}"/>.</returns>
        IEnumerable<IRedirect> GetAllRedirects();

        /// <summary>
        /// Returns an array of all rode nodes configured in Umbraco.
        /// </summary>
        /// <returns>An array of <see cref="RedirectRootNode"/> representing the root nodes.</returns>
        RedirectRootNode[] GetRootNodes();

        /// <summary>
        /// Returns an array of all rode nodes configured in Umbraco.
        /// </summary>
        /// <param name="user">An <see cref="IUser"/> with potential root node access restrictions.</param>
        /// <returns>An array of <see cref="RedirectRootNode"/> representing the root nodes the user has access to.</returns>
        RedirectRootNode[] GetRootNodes(IUser user)
        {
	        var rootNodes = GetRootNodes();
	        HashSet<int> rootNodeIds = new();

	        if (user.StartContentIds != null)
	        {
		        foreach (var rootNodeId in user.StartContentIds)
		        {
			        rootNodeIds.Add(rootNodeId);
		        }
	        }

	        foreach (var group in user.Groups)
	        {
		        if (group.StartContentId != null)
		        {
			        rootNodeIds.Add(group.StartContentId.Value);
		        }
	        }

	        return rootNodes.Where(rootNode => rootNodeIds.Any(x => rootNode.Path.Contains(x))).ToArray();
        }

        /// <summary>
        /// Returns an array of redirects where the destination matches the specified <paramref name="nodeType"/> and <paramref name="nodeId"/>.
        /// </summary>
        /// <param name="nodeType">The type of the destination node.</param>
        /// <param name="nodeId">The numeric ID of the destination node.</param>
        /// <returns>An array of <see cref="IRedirect"/>.</returns>
        IRedirect[] GetRedirectsByNodeId(RedirectDestinationType nodeType, int nodeId);

        /// <summary>
        /// Returns an array of redirects where the destination matches the specified <paramref name="nodeType"/> and <paramref name="nodeKey"/>.
        /// </summary>
        /// <param name="nodeType">The type of the destination node.</param>
        /// <param name="nodeKey">The key (GUID) of the destination node.</param>
        /// <returns>An array of <see cref="IRedirect"/>.</returns>
        IRedirect[] GetRedirectsByNodeKey(RedirectDestinationType nodeType, Guid nodeKey);

    }

}