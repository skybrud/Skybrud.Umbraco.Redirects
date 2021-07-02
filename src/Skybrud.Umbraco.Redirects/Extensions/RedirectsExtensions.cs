using System;
using Microsoft.AspNetCore.Http;

namespace Skybrud.Umbraco.Redirects.Extensions {
    
    public static class RedirectsExtensions {

        public static void Split(this string value, char separator, out string first) {
            string[] array = value?.Split(separator);
            first = array?[0];
        }

        public static void Split(this string value, char separator, out string first, out string second) {
            string[] array = value?.Split(separator);
            first = array?[0];
            second = array != null && array.Length > 1 ? array[1] : null;
        }
        
        public static Uri GetUri(this HttpRequest request) {
            return new UriBuilder {
                Scheme = request.Scheme,
                Host = request.Host.Value,
                Path = request.Path,
                Query = request.QueryString.ToUriComponent()
            }.Uri;
        }

    }

}