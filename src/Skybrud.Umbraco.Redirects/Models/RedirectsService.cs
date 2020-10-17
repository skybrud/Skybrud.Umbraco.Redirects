using System;
using System.Collections.Generic;
using System.Linq;
using Skybrud.Umbraco.Redirects.Exceptions;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using WebComposing = Umbraco.Web.Composing;
using System.Text.RegularExpressions;
using NPoco;
using Skybrud.Essentials.Time;
using Skybrud.Umbraco.Redirects.Models.Database;
using Skybrud.Umbraco.Redirects.Models.Options;
using Umbraco.Core.Composing;
using Umbraco.Core.Scoping;
using Umbraco.Core.Services;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Service for managing redirects.
    /// </summary>
    public class RedirectsService : IRedirectsService {

        private readonly IScopeProvider _scopeProvider;

        private readonly IDomainService _domains;

        private readonly ILogger _logger;

        #region Constructors

        public RedirectsService() {
            _scopeProvider = Current.ScopeProvider;
            _domains = Current.Services.DomainService;
            _logger = Current.Logger;
        }

        public RedirectsService(IScopeProvider scopeProvider, IDomainService domains, ILogger logger) {
            _scopeProvider = scopeProvider;
            _domains = domains;
            _logger = logger;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Gets an array of all domains (<see cref="RedirectDomain"/>) registered in Umbraco.
        /// </summary>
        /// <returns></returns>
        public RedirectDomain[] GetDomains() {
            return _domains.GetAll(false).Select(RedirectDomain.GetFromDomain).ToArray();
        }

        public RedirectItem AddRedirect(AddRedirectOptions model) {

            if (model == null) throw new ArgumentNullException(nameof(model));

            string url = model.OriginalUrl;
            string query = string.Empty;

            if (model.IsRegex == false) {
				string[] urlParts = url.Split('?');
                url = urlParts[0].TrimEnd('/');
				query = urlParts.Length == 2 ? urlParts[1] : string.Empty;
			}

            if (GetRedirectByUrl(model.RootNodeId, url, query) != null) {
                throw new RedirectsException("A redirect with the specified URL already exists.");
            }

            // Initialize the new redirect and populate the properties
            RedirectItem item = new RedirectItem {
                RootId = model.RootNodeId,
                RootKey = model.RootNodeKey,
                LinkId = model.Destination.Id,
                LinkKey = model.Destination.Key,
                LinkUrl = model.Destination.Url,
                LinkMode = model.Destination.Type,
                Url = url,
                QueryString = query,
                Created = EssentialsTime.UtcNow,
                Updated = EssentialsTime.UtcNow,
                IsPermanent = model.IsPermanent,
				IsRegex = model.IsRegex,
				ForwardQueryString = model.ForwardQueryString
            };

            // Attempt to add the redirect to the database
            using (IScope scope = _scopeProvider.CreateScope()) {
                try {
                    scope.Database.Insert(item.Dto);
                } catch (Exception ex) {
                    _logger.Error<RedirectsService>("Unable to insert redirect into the database", ex);
                    throw new Exception("Unable to insert redirect into the database", ex);
                }
                scope.Complete();
            }

            // Make the call to the database
            return GetRedirectById(item.Id);

        }

        /// <summary>
        /// Adds a new permanent redirect matching the specified inbound <paramref name="url"/>. A request to
        /// <paramref name="url"/> will automatically be redirected to the URL of the specified
        /// <paramref name="destionation"/> link.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The inbound URL to match.</param>
        /// <param name="destionation">An instance of <see cref="RedirectDestination"/> representing the destination link.</param>
        /// <returns>An instance of <see cref="RedirectItem"/> representing the created redirect.</returns>
        public RedirectItem AddRedirect(int rootNodeId, string url, RedirectDestination destionation) {
            return AddRedirect(rootNodeId, url, destionation, true, false, false);
        }

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
        public RedirectItem AddRedirect(int rootNodeId, string url, RedirectDestination destionation, bool permanent, bool isRegex, bool forwardQueryString) {


			var query = "";

			if (!isRegex)
			{
				string[] urlParts = url.Split('?');
				url = urlParts[0].TrimEnd('/');
				query = urlParts.Length == 2 ? urlParts[1] : "";
			}

            if (GetRedirectByUrl(rootNodeId, url, query) != null) {
                throw new RedirectsException("A redirect with the specified URL already exists.");
            }

            // Initialize the new redirect and populate the properties
            RedirectItem item = new RedirectItem {
                RootId = rootNodeId,
                LinkId = destionation.Id,
                LinkUrl = destionation.Url,
                LinkMode = destionation.Type,
                Url = url,
                QueryString = query,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                IsPermanent = permanent,
				IsRegex = isRegex,
				ForwardQueryString = forwardQueryString
            };

            using (var scope = _scopeProvider.CreateScope()) {

                var database = scope.Database;

                try {
                    // Attempt to add the redirect to the database
                    database.Insert(item.Dto);
                } catch (Exception ex) {
                    _logger.Error<RedirectsService>("Unable to insert redirect into the database", ex);
                    throw new Exception("Unable to insert redirect into the database");
                }
                scope.Complete();
            }

            // Make the call to the database
            return GetRedirectById(item.Id);

        }

        /// <summary>
        /// Saves the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirected to be saved.</param>
        /// <returns>The saved <paramref name="redirect"/>.</returns>
        public RedirectItem SaveRedirect(RedirectItem redirect) {

            // Some input validation
            if (redirect == null) throw new ArgumentNullException(nameof(redirect));

            // Check whether another redirect matches the new URL and query string
            RedirectItem existing = GetRedirectByUrl(redirect.RootId, redirect.Url, redirect.QueryString);
            if (existing != null && existing.Id != redirect.Id) {
                throw new RedirectsException("A redirect with the same URL and query string already exists.");
            }

            // Update the timestamp for when the redirect was modified
            redirect.Updated = DateTime.Now;

            // Update the redirect in the database
            using (var scope = _scopeProvider.CreateScope()) {
                try {
                    scope.Database.Update(redirect.Dto);
                } catch (Exception ex) {
                    _logger.Error<RedirectsService>("Unable to update redirect into the database", ex);
                    throw new Exception("Unable to update redirect into the database", ex);
                }
                scope.Complete();
            }

            return redirect;

        }

        /// <summary>
        /// Deletes the specified <paramref name="redirect"/> from the database.
        /// </summary>
        /// <param name="redirect">The redirected to be deleted.</param>
        public void DeleteRedirect(RedirectItem redirect) {

            // Some input validation
            if (redirect == null) throw new ArgumentNullException(nameof(redirect));

            // Remove the redirect from the database
            using (var scope = _scopeProvider.CreateScope()) {
                try {
                    scope.Database.Delete(redirect.Dto);
                } catch (Exception ex) {
                    _logger.Error<RedirectsService>("Unable to delete redirect from database", ex);
                    throw new Exception("Unable to delete redirect from database", ex);
                }
                scope.Complete();
            }

        }

        /// <summary>
        /// Gets the redirect mathing the specified numeric <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="redirectId">The numeric ID of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        public RedirectItem GetRedirectById(int redirectId) {

            // Validate the input
            if (redirectId == 0) throw new ArgumentException("redirectId must have a value", nameof(redirectId));

            RedirectItemDto dto;

            using (IScope scope = _scopeProvider.CreateScope()) {
                
                // Generate the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<RedirectItemDto>()
                    .From<RedirectItemDto>()
                    .Where<RedirectItemDto>(x => x.Id == redirectId);
                
                // Make the call to the database
                dto = scope.Database.FirstOrDefault<RedirectItemDto>(sql);
                scope.Complete();

            }

            // Wrap the database row
            return dto == null ? null : new RedirectItem(dto);

        }

        /// <summary>
        /// Gets the redirect mathing the specified GUID <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The GUID key of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        public RedirectItem GetRedirectByKey(Guid key) {

            RedirectItemDto dto;

            using (var scope = _scopeProvider.CreateScope()) {
                
                // Generate the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<RedirectItemDto>()
                    .From<RedirectItemDto>()
                    .Where<RedirectItemDto>(x => x.Key == key);
                
                // Make the call to the database
                dto = scope.Database.FirstOrDefault<RedirectItemDto>(sql);
                scope.Complete();

            }

            // Wrap the database row
            return dto == null ? null : new RedirectItem(dto);

        }

        /// <summary>
        /// Gets the redirect mathing the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The URL of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        public RedirectItem GetRedirectByUrl(int rootNodeId, string url) {

            // Some input validation
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            // Split the URL
            string[] parts = url.Split('?');
            return GetRedirectByUrl(rootNodeId, parts[0], parts.Length == 2 ? parts[1] : null);

        }
        
        /// <summary>
        /// Gets the redirect mathing the specified <paramref name="url"/> and <paramref name="queryString"/>.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The URL of the redirect.</param>
        /// <param name="queryString">The query string of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        public RedirectItem GetRedirectByUrl(int rootNodeId, string url, string queryString) {

            // Some input validation
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

			url = url.TrimEnd('/').Trim();
            queryString = (queryString ?? string.Empty).Trim();

            RedirectItemDto dto;

            using (IScope scope = _scopeProvider.CreateScope()) {

                // Generate the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<RedirectItemDto>()
                    .From<RedirectItemDto>()
                    .Where<RedirectItemDto>(x => x.RootId == rootNodeId && !x.IsRegex && x.Url == url && x.QueryString == queryString);
                
                // Make the call to the database
                dto = scope.Database.FirstOrDefault<RedirectItemDto>(sql);

                if (dto == null) {

                    // no redirect found, try with forwardQueryString = true, and no querystring
                    sql = scope.SqlContext.Sql()
                        .Select<RedirectItemDto>()
                        .From<RedirectItemDto>()
                        .Where<RedirectItemDto>(x => x.RootId == rootNodeId && x.Url == url && x.ForwardQueryString);

                    // Make the call to the database
                    dto = scope.Database.FirstOrDefault<RedirectItemDto>(sql);

                }

                scope.Complete();
            }

            // Wrap the database row
            return dto == null ? null : new RedirectItem(dto);


        }

        /// <summary>
        /// Gets the redirects mathing the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL of the redirects.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        public RedirectItem[] GetRedirectsByUrl(string url) {

            // Some input validation
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            // Split the URL
            string[] parts = url.Split('?');
            return GetRedirectsByUrl(parts[0], parts.Length == 2 ? parts[1] : null);

        }

        /// <summary>
        /// Gets the redirects mathing the specified <paramref name="url"/> and <paramref name="queryString"/>.
        /// </summary>
        /// <param name="url">The URL of the redirect.</param>
        /// <param name="queryString">The query string of the redirect.</param>
        /// <returns>An array of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        public RedirectItem[] GetRedirectsByUrl(string url, string queryString) {

            // Some input validation
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

			var fullUrl = url + (queryString.IsNullOrWhiteSpace() ? string.Empty : "?" + queryString);

			url = url.TrimEnd('/').Trim();
            queryString = (queryString ?? string.Empty).Trim();

            List<RedirectItem> redirects;

            using (IScope scope = _scopeProvider.CreateScope()) {
                
                // Fetch non-regex redirects from the database
                var sql = scope.SqlContext.Sql()
                    .Select<RedirectItemDto>()
                    .From<RedirectItemDto>()
                    .Where<RedirectItemDto>(x => !x.IsRegex && x.Url == url && (x.QueryString == queryString || x.ForwardQueryString));
                
                // Make the call to the database
                redirects = scope.Database.Fetch<RedirectItemDto>(sql)
                    .Select(RedirectItem.GetFromRow)
                    .ToList();

                // Fetch regex redirects from the database
                sql = scope.SqlContext.Sql()
                    .Select<RedirectItemDto>()
                    .From<RedirectItemDto>()
                    .Where<RedirectItemDto>(x => x.IsRegex);

                redirects.AddRange(scope.Database.Fetch<RedirectItemDto>(sql).Where(x => Regex.IsMatch(fullUrl, x.Url)).Select(RedirectItem.GetFromRow));

                scope.Complete();

            }

            // Return a combined list of the redirects
            return redirects.OrderBy(x => x.RootId > 0 ? "0" : "1").ToArray();

		}

        /// <summary>
        /// Gets an array of <see cref="RedirectItem"/> for the content item with the specified <paramref name="contentId"/>.
        /// </summary>
        /// <param name="contentId">The numeric ID of the content item.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        public RedirectItem[] GetRedirectsByContentId(int contentId) {

            RedirectItem[] rows;

            using (IScope scope = _scopeProvider.CreateScope()) {
                
                // Generate the SQL for the query
                Sql<ISqlContext> sql = scope.SqlContext.Sql()
                    .Select<RedirectItemDto>().From<RedirectItemDto>()
                    .Where<RedirectItemDto>(x => x.DestinationType == "content" && x.DestinationId == contentId);
                
                // Make the call to the database
                rows = scope.Database.Fetch<RedirectItemDto>(sql).Select(RedirectItem.GetFromRow).ToArray();

                scope.Complete();

            }

            return rows;

        }

        /// <summary>
        /// Gets an array of <see cref="RedirectItem"/> for the media item with the specified <paramref name="mediaId"/>.
        /// </summary>
        /// <param name="mediaId">The numeric ID of the media item.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        public RedirectItem[] GetRedirectsByMediaId(int mediaId) {

            RedirectItem[] rows;

            using (IScope scope = _scopeProvider.CreateScope()) {
                
                // Generate the SQL for the query
                Sql<ISqlContext> sql = scope.SqlContext.Sql()
                    .Select<RedirectItemDto>()
                    .From<RedirectItemDto>()
                    .Where<RedirectItemDto>(x => x.DestinationType == "media" && x.DestinationId == mediaId);
                
                // Make the call to the database
                rows = scope.Database.Fetch<RedirectItemDto>(sql).Select(RedirectItem.GetFromRow).ToArray();

                scope.Complete();

            }

            return rows;

        }

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
        public RedirectsSearchResult GetRedirects(int page = 1, int limit = 20, string type = null, string text = null, int? rootNodeId = null) {

            RedirectsSearchResult result;

            using (var scope = _scopeProvider.CreateScope()) {
                
                // Generate the SQL for the query
                var sql = scope.SqlContext.Sql().Select<RedirectItemDto>().From<RedirectItemDto>();

                // Search by the rootNodeId
                if (rootNodeId != null) sql = sql.Where<RedirectItemDto>(x => x.RootId == rootNodeId.Value);

                // Search by the type
                if (string.IsNullOrWhiteSpace(type) == false) sql = sql.Where<RedirectItemDto>(x => x.DestinationType == type);

                // Search by the text
                if (string.IsNullOrWhiteSpace(text) == false) {

                    string[] parts = text.Split('?');

                    if (parts.Length == 1) {
                        sql = sql.Where<RedirectItemDto>(x => x.Url.Contains(text) || x.QueryString.Contains(text));
                    } else {
                        string url = parts[0];
                        string query = parts[1];
                        sql = sql.Where<RedirectItemDto>(x => (
                            x.Url.Contains(text)
                            ||
                            (x.Url.Contains(url) && x.QueryString.Contains(query))
                        ));
                    }
                }

                // Order the redirects
                sql = sql.OrderByDescending<RedirectItemDto>(x => x.Updated);

                // Make the call to the database
                RedirectItemDto[] all = scope.Database.Fetch<RedirectItemDto>(sql).ToArray();

                // Calculate variables used for the pagination
                int pages = (int)Math.Ceiling(all.Length / (double)limit);
                page = Math.Max(1, Math.Min(page, pages));

                int offset = (page * limit) - limit;

                // Apply pagination and wrap the database rows
                RedirectItem[] items = all.Skip(offset).Take(limit).Select(RedirectItem.GetFromRow).ToArray();

                // Return the items (on the requested page)
                result = new RedirectsSearchResult(all.Length, limit, offset, page, pages, items);

                scope.Complete();

            }

            return result;

        }

	    public string HandleForwardQueryString(RedirectItem redirect, string rawurl) {

            string newRedirectUrl;

			// find querystrings from rawurl
		    string[] elementsRawurl = rawurl.Split('?');
			string querystringsRawurl = 1 < elementsRawurl.Length ? elementsRawurl[1] : null;

			if (!string.IsNullOrWhiteSpace(querystringsRawurl)) {
				
                // we have querystrings in the original url

				// find querystrings in the redirect url
			    string[] elementsRedirecturl = redirect.LinkUrl.Split('?');
			    string querystringsRedirecturl = 1 < elementsRedirecturl.Length ? elementsRedirecturl[1] : null;

				// merge querystrings
				List<string> queryElements = new List<string>();

				if (!string.IsNullOrWhiteSpace(querystringsRedirecturl))
			    {
				    queryElements.Add(querystringsRedirecturl);
			    }

				queryElements.Add(querystringsRawurl);

				// create new redirect url w. merged querystrings
			    newRedirectUrl = $"{elementsRedirecturl[0]}?{string.Join("&", queryElements)}";
		    }
		    else
		    {
			    newRedirectUrl = redirect.LinkUrl;
		    }

			return newRedirectUrl;
	    }

        #endregion

    }

}