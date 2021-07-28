using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Skybrud.Essentials.Time;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Skybrud.Umbraco.Redirects.Models.Options;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Services {
    
    public class RedirectsService : IRedirectsService {
        
        private readonly ILogger<RedirectsService> _logger;
        private readonly IScopeProvider _scopeProvider;
        private readonly IDomainService _domains;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public RedirectsService(ILogger<RedirectsService> logger, IScopeProvider scopeProvider, IDomainService domains, IUmbracoContextAccessor umbracoContextAccessor) {
            _logger = logger;
            _scopeProvider = scopeProvider;
            _domains = domains;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        /// <summary>
        /// Gets an array of all domains (<see cref="RedirectDomain"/>) registered in Umbraco.
        /// </summary>
        /// <returns></returns>
        public RedirectDomain[] GetDomains() {
            return _domains.GetAll(false).Select(RedirectDomain.GetFromDomain).ToArray();
        }







        

        /// <summary>
        /// Deletes the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirect to be deleted.</param>
        public void DeleteRedirect(Redirect redirect) {

            // Some input validation
            if (redirect == null) throw new ArgumentNullException(nameof(redirect));

            // Create a new scope
            using IScope scope = _scopeProvider.CreateScope();

            // Remove the redirect from the database
            try {
                scope.Database.Delete(redirect.Dto);
            } catch (Exception ex) {
                throw new RedirectsException("Unable to delete redirect from database.", ex);
            }

            // Complete the scope
            scope.Complete();

        }






        
        /// <summary>
        /// Gets the redirect mathing the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="rootNodeKey">The key of the root node. Use <see cref="Guid.Empty"/> for a global redirect.</param>
        /// <param name="url">The URL of the redirect.</param>
        /// <returns>An instance of <see cref="Redirect"/>, or <c>null</c> if not found.</returns>
        public virtual Redirect GetRedirectByUrl(Guid rootNodeKey, string url) {
            url.Split('?', out string path, out string query);
            return GetRedirectByPathAndQuery(rootNodeKey, path, query);
        }
        
        /// <summary>
        /// Gets the redirect mathing the specified <paramref name="path"/> and <paramref name="query"/>.
        /// </summary>
        /// <param name="rootNodeKey">The key of the root node. Use <see cref="Guid.Empty"/> for a global redirect.</param>
        /// <param name="path">The path of the redirect.</param>
        /// <param name="query">The query string of the redirect.</param>
        /// <returns>An instance of <see cref="Redirect"/>, or <c>null</c> if not found.</returns>
        public virtual Redirect GetRedirectByPathAndQuery(Guid rootNodeKey, string path, string query) {

            query ??= string.Empty;

            RedirectDto dto;
    
            using (IScope scope = _scopeProvider.CreateScope()) {
        
                // Generate the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<RedirectDto>()
                    .From<RedirectDto>()
                    .Where<RedirectDto>(x => x.RootKey == rootNodeKey && x.Path == path && x.QueryString == (query ?? string.Empty));

                // Make the call to the database
                dto = scope.Database.FirstOrDefault<RedirectDto>(sql);
                scope.Complete();

            }

            return dto == null ? null : new Redirect(dto);

        }

        public Redirect GetRedirectByRequest(HttpRequest request) {
            
            // Get the URI from the request
            Uri uri = request.GetUri();

            // Look for redirects by the URI
            return GetRedirectByUri(uri);

        }
        
        public Redirect GetRedirectByUri(Uri uri) {
            
            // Determine the root node via domain of the request
            Guid rootKey = Guid.Empty;
            if (TryGetDomain(uri, out Domain domain)) {
                IPublishedContent root = _umbracoContextAccessor.UmbracoContext.Content.GetById(domain.ContentId);
                if (root != null) rootKey = root.Key;
            }

            // Look for a site specific redirect
            if (rootKey != Guid.Empty) {
                var redirect = GetRedirectByUrl(rootKey, uri.PathAndQuery);
                if (redirect != null) return redirect;
            }
            
            // Look for a global redirect
            return GetRedirectByUrl(Guid.Empty, uri.PathAndQuery);

        }
        
        private bool TryGetDomain(Uri uri, out Domain domain) {
            domain = DomainUtils.FindDomainForUri(_domains, uri);
            return domain != null;
        }

        /// <summary>
        /// Returns the redirect with the  specified numeric <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="redirectId">The numeric ID of the redirect.</param>
        /// <returns>An instance of <see cref="Redirect"/>, or <c>null</c> if not found.</returns>
        public Redirect GetRedirectById(int redirectId) {

            // Validate the input
            if (redirectId == 0) throw new ArgumentException("redirectId must have a value", nameof(redirectId));

            RedirectDto dto;

            using (IScope scope = _scopeProvider.CreateScope()) {

                // Generate the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<RedirectDto>()
                    .From<RedirectDto>()
                    .Where<RedirectDto>(x => x.Id == redirectId);

                // Make the call to the database
                dto = scope.Database.FirstOrDefault<RedirectDto>(sql);
                scope.Complete();

            }

            // Wrap the database row
            return dto == null ? null : new Redirect(dto);

        }

        /// <summary>
        /// Gets the redirect mathing the specified GUID <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The GUID key of the redirect.</param>
        /// <returns>An instance of <see cref="Redirect"/>, or <c>null</c> if not found.</returns>
        public Redirect GetRedirectByKey(Guid key) {

            RedirectDto dto;

            using (IScope scope = _scopeProvider.CreateScope()) {

                // Generate the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<RedirectDto>()
                    .From<RedirectDto>()
                    .Where<RedirectDto>(x => x.Key == key);

                // Make the call to the database
                dto = scope.Database.FirstOrDefault<RedirectDto>(sql);
                scope.Complete();

            }

            // Wrap the database row
            return dto == null ? null : new Redirect(dto);

        }

        /// <summary>
        /// Gets the redirect mathing the specified <paramref name="url"/> and <paramref name="queryString"/>.
        /// </summary>
        /// <param name="rootNodeKey">The key of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The URL of the redirect.</param>
        /// <param name="queryString">The query string of the redirect.</param>
        /// <returns>An instance of <see cref="Redirect"/>, or <c>null</c> if not found.</returns>
        public Redirect GetRedirectByUrl(Guid rootNodeKey, string url, string queryString) {

            // Some input validation
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

            url = url.TrimEnd('/').Trim();
            queryString = (queryString ?? string.Empty).Trim();

            RedirectDto dto;

            using (IScope scope = _scopeProvider.CreateScope()) {

                // Generate the SQL for the query
                var sql = scope.SqlContext.Sql()
                    .Select<RedirectDto>()
                    .From<RedirectDto>()
                    .Where<RedirectDto>(x => x.RootKey == rootNodeKey && !x.IsRegex && x.Path == url && x.QueryString == queryString);

                // Make the call to the database
                dto = scope.Database.FirstOrDefault<RedirectDto>(sql);

                if (dto == null) {

                    // no redirect found, try with forwardQueryString = true, and no querystring
                    sql = scope.SqlContext.Sql()
                        .Select<RedirectDto>()
                        .From<RedirectDto>()
                        .Where<RedirectDto>(x => x.RootKey == rootNodeKey && x.Path == url && x.ForwardQueryString);

                    // Make the call to the database
                    dto = scope.Database.FirstOrDefault<RedirectDto>(sql);

                }

                scope.Complete();

            }

            // Wrap the database row
            return dto == null ? null : new Redirect(dto);

        }

        /// <inheritdoc />
        public Redirect AddRedirect(AddRedirectOptions options) {

            if (options == null) throw new ArgumentNullException(nameof(options));

            string url = options.OriginalUrl;
            string query = string.Empty;

            if (GetRedirectByUrl(options.RootNodeKey, url, query) != null) {
                throw new RedirectsException("A redirect with the specified URL already exists.");
            }

            // Initialize the destination
            RedirectDestination destination = new RedirectDestination {
                Id = options.Destination.Id,
                Key = options.Destination.Key,
                Name = options.Destination.Name,
                Url = options.Destination.Url,
                Type = options.Destination.Type
            };

            // Initialize the new redirect and populate the properties
            Redirect item = new Redirect {
                RootKey = options.RootNodeKey,
                Url = url,
                QueryString = query,
                CreateDate = EssentialsTime.UtcNow,
                UpdateDate = EssentialsTime.UtcNow,
                IsPermanent = options.IsPermanent,
                ForwardQueryString = options.ForwardQueryString
            }.SetDestination(destination);

            // Attempt to add the redirect to the database
            using (IScope scope = _scopeProvider.CreateScope()) {
                try {
                    scope.Database.Insert(item.Dto);
                } catch (Exception ex) {
                    //_logger.Error<RedirectsService>("Unable to insert redirect into the database", ex);
                    throw new RedirectsException("Unable to insert redirect into the database.", ex);
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
        public Redirect SaveRedirect(Redirect redirect) {

            // Some input validation
            if (redirect == null) throw new ArgumentNullException(nameof(redirect));

            // Check whether another redirect matches the new URL and query string
            Redirect existing = GetRedirectByUrl(redirect.RootKey, redirect.Url, redirect.QueryString);
            if (existing != null && existing.Id != redirect.Id) {
                throw new RedirectsException("A redirect with the same URL and query string already exists.");
            }

            // Update the timestamp for when the redirect was modified
            redirect.UpdateDate = DateTime.UtcNow;

            // Update the redirect in the database
            using (var scope = _scopeProvider.CreateScope()) {
                try {
                    scope.Database.Update(redirect.Dto);
                } catch (Exception ex) {
                    _logger.LogError(ex, "Unable to update redirect into the database.");
                    throw new RedirectsException("Unable to update redirect into the database.", ex);
                }
                scope.Complete();
            }

            return redirect;

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
                var sql = scope.SqlContext.Sql().Select<RedirectDto>().From<RedirectDto>();

                // Search by the rootNodeId
                if (rootNodeId != null) sql = sql.Where<RedirectDto>(x => x.RootId == rootNodeId.Value);

                // Search by the type
                if (string.IsNullOrWhiteSpace(type) == false) sql = sql.Where<RedirectDto>(x => x.DestinationType == type);

                // Search by the text
                if (string.IsNullOrWhiteSpace(text) == false) {

                    string[] parts = text.Split('?');

                    if (parts.Length == 1) {
                        sql = sql.Where<RedirectDto>(x => x.Path.Contains(text) || x.QueryString.Contains(text));
                    } else {
                        string url = parts[0];
                        string query = parts[1];
                        sql = sql.Where<RedirectDto>(x => (
                            x.Path.Contains(text)
                            ||
                            (x.Path.Contains(url) && x.QueryString.Contains(query))
                        ));
                    }
                }

                // Order the redirects
                sql = sql.OrderByDescending<RedirectDto>(x => x.Updated);

                // Make the call to the database
                RedirectDto[] all = scope.Database.Fetch<RedirectDto>(sql).ToArray();

                // Calculate variables used for the pagination
                int pages = (int) Math.Ceiling(all.Length / (double)limit);
                page = Math.Max(1, Math.Min(page, pages));

                int offset = (page * limit) - limit;

                // Apply pagination and wrap the database rows
                Redirect[] items = all.Skip(offset).Take(limit).Select(Redirect.CreateFromDto).ToArray();

                // Return the items (on the requested page)
                result = new RedirectsSearchResult(all.Length, limit, offset, page, pages, items);

                scope.Complete();

            }

            return result;

        }

        public IEnumerable<Redirect> GetAllRedirects()  {
            
            // Create a new scope
            using var scope = _scopeProvider.CreateScope();
            
            // Generate the SQL for the query
            var sql = scope.SqlContext.Sql().Select<RedirectDto>().From<RedirectDto>();
                
            // Make the call to the database
            var redirects = scope.Database.Fetch<RedirectDto>(sql).Select(Redirect.CreateFromDto);
                
            scope.Complete();

            return redirects;

        }

        
        
        public virtual string GetDestinationUrl(Redirect redirect) {

            string url = redirect.Destination.Url;

            switch (redirect.Destination.Type) {

                case RedirectDestinationType.Content:
                    IPublishedContent content = _umbracoContextAccessor.UmbracoContext.Content.GetById(redirect.Destination.Key);
                    if (content != null) return content.Url();
                    break;

                case RedirectDestinationType.Media:
                    IPublishedContent media = _umbracoContextAccessor.UmbracoContext.Media.GetById(redirect.Destination.Key);
                    if (media != null) return media.Url();
                    break;

            }

            return url;

        }

        public virtual string GetDestinationUrl(Redirect redirect, string inboundUrl) {

            // Resolve the current URL for content and media
            string destinationUrl = redirect.Destination.Url;
            
            switch (redirect.Destination.Type) {
                
                case RedirectDestinationType.Content:
                    var content = _umbracoContextAccessor.UmbracoContext.Content.GetById(redirect.Destination.Key);
                    if (content != null) destinationUrl = content.Url();
                    break;

                case RedirectDestinationType.Media:
                    var media = _umbracoContextAccessor.UmbracoContext.Media.GetById(redirect.Destination.Key);
                    if (media != null) destinationUrl = media.Url();
                    break;
  
            }

            return redirect.ForwardQueryString ? MergeQueryString(inboundUrl, destinationUrl) : destinationUrl;

        }

        protected virtual string MergeQueryString(string inboundUrl, string destinationUrl) {
            
            // Split the inbound URL
            inboundUrl.Split('?', out string _, out string inboundQuery);
            if (string.IsNullOrWhiteSpace(inboundQuery)) return destinationUrl;

            // Split the destination URL
            destinationUrl.Split('?', out string destinationPath, out string destinationQuery);

            // Merge the two query strings
            List<string> queryElements = new List<string>();
            if (!string.IsNullOrWhiteSpace(destinationQuery)) {
                queryElements.Add(destinationQuery);
            }
            queryElements.Add(inboundQuery);

            // Put the URL back together
            return $"{destinationPath}?{string.Join("&", queryElements)}";

        }

    }

}