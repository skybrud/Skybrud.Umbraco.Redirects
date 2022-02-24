using Skybrud.Essentials.Reflection;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Cms.Core.Semver;

namespace Skybrud.Umbraco.Redirects {

    internal class RedirectsUtils {

        /// <summary>
        /// Returns the concatenated URL based on <paramref name="url"/> and <paramref name="query"/>.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="query">The query string.</param>
        /// <returns>The combined URL.</returns>
        public static string ConcatUrl(string url, string query) {
            return $"{url}{(string.IsNullOrWhiteSpace(query) ? null : "?" + query)}";
        }

        /// <summary>
        /// Returns the concatenated URL based on <paramref name="url"/>, <paramref name="query"/> and <paramref name="fragment"/>.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="query">The query string.</param>
        /// <param name="fragment">The fragment.</param>
        /// <returns>The combined URL.</returns>
        public static string ConcatUrl(string url, string query, string fragment) {
            return $"{url}{(string.IsNullOrWhiteSpace(query) ? null : "?" + query)}{fragment}";
        }

        public static bool NormalizeUrlParts(Redirect redirect) {

            if (redirect.Destination is null) return false;

            string url = redirect.Destination.Url;
            string query = redirect.Destination.Query;
            string fragment = null;

            // Isolate the fragment if specified in the URL
            int pos1 = url.IndexOf('#');
            if (pos1 >= 0) {
                fragment = url.Substring(pos1);
                url = url.Substring(0, pos1);
            }

            // Isolate the query string if specified in the URL
            int pos2 = url.IndexOf('?');
            if (pos2 >= 0) {
                query += "&" + url.Substring(pos2 + 1);
                url = url.Substring(0, pos2);
            }

            // Parse the "fragment" value
            if (redirect.Destination.Fragment.HasValue()) {

                string temp = redirect.Destination.Fragment;

                // Isolate the fragment if specified in the "anchor" value (overwrites fragment from the URL)
                var pos3 = temp.IndexOf('#');
                if (pos3 >= 0) {
                    fragment = temp.Substring(pos3);
                    temp = pos3 > 0 ? temp.Substring(0, pos3 - 1) : string.Empty;
                }

                // Treat remaining anchor value as query string (append if URL also has query string)
                if (temp.HasValue()) {
                    if (temp.IndexOf('?') == 0 || temp.IndexOf('&') == 0) {
                        query += "&" + temp.Substring(1);
                    } else {
                        query += "&" + temp;
                    }
                }

            }

            string linkUrl = url;
            string linkQuery = query == null ? string.Empty : query.TrimStart('&');
            string linkFragment = fragment ?? string.Empty;

            bool hasChanges = (redirect.Destination.Url != linkUrl || redirect.Destination.Query != linkQuery || redirect.Destination.Fragment != linkFragment);

            redirect.Destination.Url = linkUrl;
            redirect.Destination.Query = linkQuery;
            redirect.Destination.Fragment = linkFragment;

            return hasChanges;

        }

        public static SemVersion GetSemVersion() {
            return SemVersion.Parse(ReflectionUtils.GetInformationalVersion(typeof(RedirectsUtils).Assembly));
        }

    }

}