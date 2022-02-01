using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects {
    
    internal class RedirectsUtils {

        public static bool NormalizeUrlParts(RedirectItem redirect) {

            string url = redirect.LinkUrl;
            string query = null;
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
                query = url.Substring(pos2 + 1);
                url = url.Substring(0, pos2);
            }

            // Parse the "fragment" value
            if (redirect.LinkFragment.HasValue()) {

                // Isolate the fragment if specified in the "anchor" value (overwrites fragment from the URL)
                var pos3 = redirect.LinkFragment.IndexOf('#');
                if (pos3 >= 0) {
                    fragment = redirect.LinkFragment.Substring(pos3);
                }

                // Treat remaining anchor value as query string (append if URL also has query string)
                if (redirect.LinkFragment.HasValue()) {
                    if (redirect.LinkFragment.IndexOf('?') == 0 || redirect.LinkFragment.IndexOf('&') == 0) {
                        query += "&" + redirect.LinkFragment.Substring(1);
                    } else {
                        query += "&" + redirect.LinkFragment;
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