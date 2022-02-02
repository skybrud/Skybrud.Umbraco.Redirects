using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects {

    internal class RedirectsUtils {

        public static bool NormalizeUrlParts(RedirectItem redirect) {

            string url = redirect.LinkUrl;
            string query = redirect.LinkQuery;
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
            if (redirect.LinkFragment.HasValue()) {

                string temp = redirect.LinkFragment;

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

            bool hasChanges = (redirect.LinkUrl != linkUrl || redirect.LinkQuery != linkQuery || redirect.LinkFragment != linkFragment);

            redirect.LinkUrl = linkUrl;
            redirect.LinkQuery = linkQuery;
            redirect.LinkFragment = linkFragment;

            return hasChanges;

        }

    }

}