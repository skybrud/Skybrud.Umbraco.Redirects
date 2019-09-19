using System;
using Skybrud.Umbraco.Redirects.Models.Options;

namespace Skybrud.Umbraco.Redirects.Models {

    public interface IRedirectsService {

        /// <summary>
        /// Returns an array of <see cref="RedirectDomain"/> representing all the domains registered in Umbraco.
        /// </summary>
        /// <returns>An array of <see cref="RedirectDomain"/>.</returns>
        RedirectDomain[] GetDomains();

        RedirectItem AddRedirect(AddRedirectOptions rootNodeId);

        /// <summary>
        /// Adds a new permanent redirect matching the specified inbound <paramref name="url"/>. A request to
        /// <paramref name="url"/> will automatically be redirected to the URL of the specified
        /// <paramref name="destionation"/> link.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The inbound URL to match.</param>
        /// <param name="destionation">An instance of <see cref="RedirectDestination"/> representing the destination link.</param>
        /// <returns>An instance of <see cref="RedirectItem"/> representing the created redirect.</returns>
        RedirectItem AddRedirect(int rootNodeId, string url, RedirectDestination destionation);

        /// <summary>
        /// Adds a new redirect matching the specified inbound <paramref name="url"/>. A request to
        /// <paramref name="url"/> will automatically be redirected to the URL of the specified
        /// <paramref name="destionation"/> link.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The inbound URL to match.</param>
        /// <param name="destionation">An instance of <see cref="RedirectDestination"/> representing the destination link.</param>
        /// <param name="permanent">Whether the redirect should be permanent (301) or temporary (302).</param>
        /// <param name="isRegex">Whether regex should be enabled for the redirect.</param>
        /// <param name="forwardQueryString">Whether the redirect should forward the original query string.</param>
        /// <returns>An instance of <see cref="RedirectItem"/> representing the created redirect.</returns>
        RedirectItem AddRedirect(int rootNodeId, string url, RedirectDestination destionation, bool permanent, bool isRegex, bool forwardQueryString);

        /// <summary>
        /// Saves the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirected to be saved.</param>
        /// <returns>The saved <paramref name="redirect"/>.</returns>
        RedirectItem SaveRedirect(RedirectItem redirect);

        /// <summary>
        /// Deletes the specified <paramref name="redirect"/> from the database.
        /// </summary>
        /// <param name="redirect">The redirected to be deleted.</param>
        void DeleteRedirect(RedirectItem redirect);

        /// <summary>
        /// Gets the redirect mathing the specified numeric <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="redirectId">The numeric ID of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        RedirectItem GetRedirectById(int redirectId);

        /// <summary>
        /// Gets the redirect mathing the specified GUID <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The GUID key of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        RedirectItem GetRedirectByKey(Guid key);

        /// <summary>
        /// Gets the redirect mathing the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The URL of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        RedirectItem GetRedirectByUrl(int rootNodeId, string url);

        /// <summary>
        /// Gets the redirect mathing the specified <paramref name="url"/> and <paramref name="queryString"/>.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The URL of the redirect.</param>
        /// <param name="queryString">The query string of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        RedirectItem GetRedirectByUrl(int rootNodeId, string url, string queryString);

        /// <summary>
        /// Gets the redirects mathing the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL of the redirects.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        RedirectItem[] GetRedirectsByUrl(string url);

        /// <summary>
        /// Gets the redirects mathing the specified <paramref name="url"/> and <paramref name="queryString"/>.
        /// </summary>
        /// <param name="url">The URL of the redirect.</param>
        /// <param name="queryString">The query string of the redirect.</param>
        /// <returns>An array of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        RedirectItem[] GetRedirectsByUrl(string url, string queryString);

        /// <summary>
        /// Gets an array of <see cref="RedirectItem"/> for the content item with the specified <paramref name="contentId"/>.
        /// </summary>
        /// <param name="contentId">The numeric ID of the content item.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        RedirectItem[] GetRedirectsByContentId(int contentId);

        /// <summary>
        /// Gets an array of <see cref="RedirectItem"/> for the media item with the specified <paramref name="mediaId"/>.
        /// </summary>
        /// <param name="mediaId">The numeric ID of the media item.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        RedirectItem[] GetRedirectsByMediaId(int mediaId);

        /// <summary>
        /// Gets an instance of <see cref="RedirectsSearchResult"/> representing a paginated search for redirects.
        /// </summary>
        /// <param name="page">The page to be returned (default is <c>1</c>)</param>
        /// <param name="limit">The maximum amount of redirects to be returned per page (default is <c>20</c>).</param>
        /// <param name="type">The type of the redirects to be returned. Possible values are <c>url</c>,
        ///     <c>content</c> or <c>media</c>. If not specified, all types of redirects will be returned.
        ///     Default is <c>null</c>.</param>
        /// <param name="text">A string value that should be present in either the text or URL of the returned
        ///     redirects. Default is <c>null</c>.</param>
        /// <param name="rootNodeId"></param>
        /// <returns>An instance of <see cref="RedirectsSearchResult"/>.</returns>
        RedirectsSearchResult GetRedirects(int page = 1, int limit = 20, string type = null, string text = null, int? rootNodeId = null);

        string HandleForwardQueryString(RedirectItem redirect, string rawurl);

    }

}