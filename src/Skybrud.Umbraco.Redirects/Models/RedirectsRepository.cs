using System;
using System.Linq;
using Skybrud.Umbraco.Redirects.Exceptions;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;

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
        public UmbracoDatabase Database {
            get { return ApplicationContext.Current.DatabaseContext.Database; }
        }

        protected ISqlSyntaxProvider SqlSyntax {
            get { return ApplicationContext.Current.DatabaseContext.SqlSyntax; }
        }

        protected readonly DatabaseSchemaHelper SchemaHelper = new DatabaseSchemaHelper(
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
        /// Adds a new redirect matching the specified <code>url</code>. A user matching <code>url</code> will
        /// automatically be redirected to the URL of the content node with the specified <code>contentId</code>.
        /// </summary>
        /// <param name="url">The URL to match.</param>
        /// <param name="redirect">The ID of the content node the user should be redirected to.</param>
        /// <returns>Returns an instance of <see cref="RedirectItem"/> representing the created redirect.</returns>
        public RedirectItem AddRedirect(string url, RedirectLinkItem redirect) {
            return AddRedirect(url, redirect, true);
        }

        public RedirectItem AddRedirect(string url, RedirectLinkItem redirect, bool permanent) {

            // Attempt to create the database table if it doesn't exist
            //try {
                if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) {
                    SchemaHelper.CreateTable<RedirectItemRow>(false);
                }
            //} catch (Exception ex) {
            //    LogHelper.Error<RedirectsRepository>("Unable to create database table: " + RedirectItem.TableName, ex);
            //    throw new Exception("Din opgave kunne ikke oprettes pga. en fejl på serveren");
            //}

            string[] urlParts = url.Split('?');
            url = urlParts[0].TrimEnd('/');
            string query = urlParts.Length == 2 ? urlParts[1] : "";

            if (GetRedirectByUrl(url, query) != null) {
                throw new RedirectsException("A redirect with the specified URL already exists.");
            }

            // Initialize the new redirect and populate the properties
            RedirectItem item = new RedirectItem {
                LinkId = redirect.Id,
                LinkUrl = redirect.Url,
                LinkMode = redirect.Mode,
                LinkName = redirect.Name,
                Url = url,
                QueryString = query,
                Created = DateTime.Now,
                Updated = DateTime.Now,
                IsPermanent = permanent
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

        public RedirectItem SaveRedirect(RedirectItem redirect) {

            // Some input validation
            if (redirect == null) throw new ArgumentNullException("redirect");

            // Check whether another redirect matches the new URL and query string
            RedirectItem existing = GetRedirectByUrl(redirect.Url, redirect.QueryString);
            if (existing != null && existing.Id != redirect.Id) {
                throw new RedirectsException("A redirect with the same URL and query string already exists.");
            }

            // Update the timestamp for when the redirect was modified
            redirect.Updated = DateTime.Now;

            // Update the redirect in the database
            Database.Update(redirect.Row);

            return redirect;

        }

        public void DeleteRedirect(RedirectItem redirect) {

            // Some input validation
            if (redirect == null) throw new ArgumentNullException("redirect");

            // Remove the redirect from the database
            Database.Delete(redirect.Row);

        }

        public RedirectItem GetRedirectById(int redirectId) {

            // Validate the input
            if (redirectId == 0) throw new ArgumentException("redirectId");

            // Just return "null" if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return null;

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.Id == redirectId);

            // Make the call to the database
            RedirectItemRow row = Database.FirstOrDefault<RedirectItemRow>(sql);

            // Wrap the database row
            return row == null ? null : new RedirectItem(row);

        }

        public RedirectItem GetRedirectById(string redirectId) {

            // Validate the input
            if (String.IsNullOrWhiteSpace(redirectId)) throw new ArgumentException("redirectId");

            // Just return "null" if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return null;

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.UniqueId == redirectId);

            // Make the call to the database
            RedirectItemRow row = Database.FirstOrDefault<RedirectItemRow>(sql);

            // Wrap the database row
            return row == null ? null : new RedirectItem(row);

        }

        public RedirectItem GetRedirectByUrl(string url) {

            // Some input validation
            if (String.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");

            // Split the URL
            string[] parts = url.Split('?');
            return GetRedirectByUrl(parts[0], parts.Length == 2 ? parts[1] : null);

        }

        public RedirectItem GetRedirectByUrl(string url, string queryString) {

            // Some input validation
            if (String.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");

            url = url.TrimEnd('/').Trim();
            queryString = (queryString ?? "").Trim();

            // Just return "null" if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return null;

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.Url == url && x.QueryString == queryString);

            // Make the call to the database
            RedirectItemRow row = Database.FirstOrDefault<RedirectItemRow>(sql);

            // Wrap the database row
            return row == null ? null : new RedirectItem(row);

        }

        public RedirectItem[] GetRedirectsByContentId(int contentId) {

            // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectItem[0];

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.LinkMode == "content" && x.LinkId == contentId);

            // Make the call to the database
            return Database.Fetch<RedirectItemRow>(sql).Select(RedirectItem.GetFromRow).ToArray();

        }

        public RedirectsSearchResult GetRedirects(int page = 1, int limit = 20, string type = null, string text = null) {

            // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectsSearchResult(0, limit, 0, 0, 0, new RedirectItem[0]);

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName);

            // Search by the type
            if (!String.IsNullOrWhiteSpace(type)) sql = sql.Where<RedirectItemRow>(x => x.LinkMode == type);

            // Search by the text
            if (!String.IsNullOrWhiteSpace(text)) sql = sql.Where<RedirectItemRow>(x => x.LinkName.Contains(text) || x.Url.Contains(text));
            
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

        public object GetRedirectsForContent(int contentId) {

            // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectItem[0];

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.LinkId == contentId && x.LinkMode == "content");

            // Make the call to the database
            return Database.Fetch<RedirectItemRow>(sql).Select(RedirectItem.GetFromRow).ToArray();

        }

        public object GetRedirectsForMedia(int mediaId) {

            // Just return an empty array if the table doesn't exist (since there aren't any redirects anyway)
            if (!SchemaHelper.TableExist(RedirectItemRow.TableName)) return new RedirectItem[0];

            // Generate the SQL for the query
            Sql sql = new Sql().Select("*").From(RedirectItemRow.TableName).Where<RedirectItemRow>(x => x.LinkId == mediaId && x.LinkMode == "media");

            // Make the call to the database
            return Database.Fetch<RedirectItemRow>(sql).Select(RedirectItem.GetFromRow).ToArray();

        }

        #endregion

    }

}