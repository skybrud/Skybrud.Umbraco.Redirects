using System;
using System.Collections.Generic;
using System.Linq;
using Skybrud.Umbraco.Redirects.Exceptions;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using System.Text.RegularExpressions;

namespace Skybrud.Umbraco.Redirects.Models {

    public class RedirectsRepository {

        #region Properties

        public static RedirectsRepository Current {
            // TODO: Implement as singleton or similar
            get { return new RedirectsRepository(); }
        }

        /// <summary>
        /// Gets a reference to the Umbraco database.
        /// </summary>
        public UmbracoDatabase Database => ApplicationContext.Current.DatabaseContext.Database;

        protected ISqlSyntaxProvider SqlSyntax => ApplicationContext.Current.DatabaseContext.SqlSyntax;

        protected DatabaseSchemaHelper SchemaHelper => new DatabaseSchemaHelper(
            ApplicationContext.Current.DatabaseContext.Database,
            ApplicationContext.Current.ProfilingLogger.Logger,
            ApplicationContext.Current.DatabaseContext.SqlSyntax
        );

        #endregion

        #region Member methods

        /// <summary>
        /// Gets an array of all domains (<see cref="RedirectDomain"/>) registered in Umbraco.
        /// </summary>
        /// <returns></returns>
        public RedirectDomain[] GetDomains() {
            return ApplicationContext.Current.Services.DomainService.GetAll(false).Select(RedirectDomain.GetFromDomain).ToArray();
        }

        /// <summary>
        /// Adds a new permanent redirect matching the specified inbound <paramref name="url"/>. A request to
        /// <paramref name="url"/> will automatically be redirected to the URL of the specified
        /// <paramref name="destionation"/> link.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The inbound URL to match.</param>
        /// <param name="destionation">An instance of <see cref="RedirectLinkItem"/> representing the destination link.</param>
        /// <returns>An instance of <see cref="RedirectItem"/> representing the created redirect.</returns>
        public RedirectItem AddRedirect(int rootNodeId, string url, RedirectLinkItem destionation) {
            return AddRedirect(rootNodeId, url, destionation, true, false, false);
        }

        /// <summary>
        /// Adds a new redirect matching the specified inbound <paramref name="url"/>. A request to
        /// <paramref name="url"/> will automatically be redirected to the URL of the specified
        /// <paramref name="destionation"/> link.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The inbound URL to match.</param>
        /// <param name="destionation">An instance of <see cref="RedirectLinkItem"/> representing the destination link.</param>
        /// <param name="permanent">Whether the redirect should be permanent (301) or temporary (302).</param>
        /// <param name="isRegex">Whether regex should be enabled for the redirect.</param>
        /// <param name="forwardQueryString">Whether the redirect should forward the original query string.</param>
        /// <returns>An instance of <see cref="RedirectItem"/> representing the created redirect.</returns>
        public RedirectItem AddRedirect(int rootNodeId, string url, RedirectLinkItem destionation, bool permanent, bool isRegex, bool forwardQueryString) {

            // Attempt to create the database table if it doesn't exist
            //try {
                if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) {
                    SchemaHelper.CreateTable<RedirectItemRow>(false);
                }
			//} catch (Exception ex) {
			//    LogHelper.Error<RedirectsRepository>("Unable to create database table: " + RedirectItem.TableName, ex);
			//    throw new Exception("Din opgave kunne ikke oprettes pga. en fejl på serveren");
			//}

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
                RootNodeId = rootNodeId,
                LinkId = destionation.Id,
                LinkUrl = destionation.Url,
                LinkMode = destionation.Mode,
                LinkName = destionation.Name,
                Url = url,
                QueryString = query,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                IsPermanent = permanent,
				IsRegex = isRegex,
				ForwardQueryString = forwardQueryString
            };

            // Attempt to add the redirect to the database
            try {
                Database.Insert(item.Row);
            } catch (Exception ex) {
                LogHelper.Error<RedirectsRepository>("Unable to insert redirect into the database", ex);
                throw new Exception("Unable to insert redirect into the database");
            }

            // Make the call to the database
            return GetRedirectById(item.Id);

        }

        /// <summary>
        /// Saves the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirected to be saved.</param>
        /// <returns>The saved <see cref="redirect"/>.</returns>
        public RedirectItem SaveRedirect(RedirectItem redirect) {

            // Some input validation
            if (redirect == null) throw new ArgumentNullException(nameof(redirect));

            // Check whether another redirect matches the new URL and query string
            RedirectItem existing = GetRedirectByUrl(redirect.RootNodeId, redirect.Url, redirect.QueryString);
            if (existing != null && existing.Id != redirect.Id) {
                throw new RedirectsException("A redirect with the same URL and query string already exists.");
            }

            // Update the timestamp for when the redirect was modified
            redirect.Updated = DateTime.Now;

            // Update the redirect in the database
            Database.Update(redirect.Row);

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
            Database.Delete(redirect.Row);

        }

        /// <summary>
        /// Gets the redirect mathing the specified numeric <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="redirectId">The numeric ID of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        public RedirectItem GetRedirectById(int redirectId) {

            // Validate the input
            if (redirectId == 0) throw new ArgumentException("redirectId must have a value", nameof(redirectId));

            // Just return "null" if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return null;

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.Id == redirectId);

            // Make the call to the database
            RedirectItemRow row = Database.FirstOrDefault<RedirectItemRow>(sql);

            // Wrap the database row
            return row == null ? null : new RedirectItem(row);

        }

        /// <summary>
        /// Gets the redirect mathing the specified unique <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="redirectId">The unique ID of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        public RedirectItem GetRedirectById(string redirectId) {

            // Validate the input
            if (String.IsNullOrWhiteSpace(redirectId)) throw new ArgumentNullException(nameof(redirectId));

            // Just return "null" if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return null;

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.UniqueId == redirectId);

            // Make the call to the database
            RedirectItemRow row = Database.FirstOrDefault<RedirectItemRow>(sql);

            // Wrap the database row
            return row == null ? null : new RedirectItem(row);

        }

        /// <summary>
        /// Gets the redirect mathing the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="rootNodeId">THe ID of the root/side node. Use <c>0</c> for a global redirect.</param>
        /// <param name="url">The URL of the redirect.</param>
        /// <returns>An instance of <see cref="RedirectItem"/>, or <c>null</c> if not found.</returns>
        public RedirectItem GetRedirectByUrl(int rootNodeId, string url) {

            // Some input validation
            if (String.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

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
            if (String.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

			url = url.TrimEnd('/').Trim();
            queryString = (queryString ?? "").Trim();

            // Just return "null" if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return null;

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.RootNodeId == rootNodeId && !x.IsRegex && x.Url == url && x.QueryString == queryString);

            // Make the call to the database
            RedirectItemRow row = Database.FirstOrDefault<RedirectItemRow>(sql);

			if (row == null) {
 				
                // no redirect found, try with forwardQueryString = true, and no querystring
 				sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.RootNodeId == rootNodeId && x.Url == url && x.ForwardQueryString);
 
 				// Make the call to the database
 				row = Database.FirstOrDefault<RedirectItemRow>(sql);
 			
            }

            // Wrap the database row
            return row == null ? null : new RedirectItem(row);


        }

        /// <summary>
        /// Gets the redirects mathing the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="url">The URL of the redirects.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        public RedirectItem[] GetRedirectsByUrl(string url) {

            // Some input validation
            if (String.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

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
            if (String.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));

			var fullUrl = url + (queryString.IsNullOrWhiteSpace() ? "" : "?" + queryString);

			url = url.TrimEnd('/').Trim();
            queryString = (queryString ?? "").Trim();

            // Just return "null" if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return null;

            // Fetch non-regex redirects from the database
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => !x.IsRegex && x.Url == url && x.QueryString == queryString || x.ForwardQueryString);
            List<RedirectItem> redirects = Database.Fetch<RedirectItemRow>(sql).Select(RedirectItem.GetFromRow).ToList();

            // Fetch regex redirects from the database
            sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.IsRegex);
            redirects.AddRange(Database.Fetch<RedirectItemRow>(sql).Where(x => Regex.IsMatch(fullUrl, x.Url)).Select(RedirectItem.GetFromRow));

            // Return a combined list of the redirects
            return redirects.OrderBy(x => x.RootNodeId > 0 ? "0" : "1").ToArray();

		}

        /// <summary>
        /// Gets an array of <see cref="RedirectItem"/> for the content item with the specified <paramref name="contentId"/>.
        /// </summary>
        /// <param name="contentId">The numeric ID of the content item.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        public RedirectItem[] GetRedirectsByContentId(int contentId) {

            // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectItem[0];

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.LinkMode == "content" && x.LinkId == contentId);

            // Make the call to the database
            return Database.Fetch<RedirectItemRow>(sql).Select(RedirectItem.GetFromRow).ToArray();

        }

        /// <summary>
        /// Gets an array of <see cref="RedirectItem"/> for the media item with the specified <paramref name="mediaId"/>.
        /// </summary>
        /// <param name="mediaId">The numeric ID of the media item.</param>
        /// <returns>An array of <see cref="RedirectItem"/>.</returns>
        public RedirectItem[] GetRedirectsByMediaId(int mediaId) {

            // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectItem[0];

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.LinkMode == "media" && x.LinkId == mediaId);

            // Make the call to the database
            return Database.Fetch<RedirectItemRow>(sql).Select(RedirectItem.GetFromRow).ToArray();

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

            // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectsSearchResult(0, limit, 0, 0, 0, new RedirectItem[0]);

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName);

            // Search by the rootNodeId
            if (rootNodeId != null) sql = sql.Where<RedirectItemRow>(x => x.RootNodeId == rootNodeId.Value);

            // Search by the type
            if (!String.IsNullOrWhiteSpace(type)) sql = sql.Where<RedirectItemRow>(x => x.LinkMode == type);

            // Search by the text
            if (!String.IsNullOrWhiteSpace(text)) {
                string[] parts = text.Split('?');
                if (parts.Length == 1) {
                    sql = sql.Where<RedirectItemRow>(x => x.LinkName.Contains(text) || x.Url.Contains(text) || x.QueryString.Contains(text));
                } else {
                    string url = parts[0];
                    string query = parts[1];
                    sql = sql.Where<RedirectItemRow>(x => (
                        x.LinkName.Contains(text)
                        ||
                        x.Url.Contains(text)
                        ||
                        (x.Url.Contains(url) && x.QueryString.Contains(query))
                    ));
                }
            }
            
            // Order the redirects
            sql = sql.OrderByDescending<RedirectItemRow>(x => x.Updated, SqlSyntax);

            // Make the call to the database
            RedirectItemRow[] all = Database.Fetch<RedirectItemRow>(sql).ToArray();

            // Calculate variables used for the pagination
            int pages = (int) Math.Ceiling(all.Length / (double) limit);
            page = Math.Max(1, Math.Min(page, pages));

            int offset = (page * limit) - limit;

            // Apply pagination and wrap the database rows
            RedirectItem[] items = all.Skip(offset).Take(limit).Select(RedirectItem.GetFromRow).ToArray();

            // Return the items (on the requested page)
            return new RedirectsSearchResult(all.Length, limit, offset, page, pages, items);

        }

        //public object GetRedirectsForContent(int contentId) {

        //    // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
        //    if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectItem[0];

        //    // Generate the SQL for the query
        //    Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.LinkId == contentId && x.LinkMode == "content");

        //    // Make the call to the database
        //    return Database.Fetch<RedirectItemRow>(sql).Select(RedirectItem.GetFromRow).ToArray();

        //}

        //public object GetRedirectsForMedia(int mediaId) {

        //    // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
        //    if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectItem[0];

        //    // Generate the SQL for the query
        //    Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.LinkId == mediaId && x.LinkMode == "media");

        //    // Make the call to the database
        //    return Database.Fetch<RedirectItemRow>(sql).Select(RedirectItem.GetFromRow).ToArray();

        //}

        #endregion

    }

}