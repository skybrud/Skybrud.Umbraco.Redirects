using System;
using Microsoft.AspNetCore.Http;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

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
                Host = request.Host.Host,
                Port = request.Host.Port ?? (request.Scheme == "https" ? 80 : 443),
                Path = request.Path,
                Query = request.QueryString.ToUriComponent()
            }.Uri;
        }
        
        /// <summary>
        /// Sets the destination of <paramref name="redirect"/> to the specified <paramref name="url"/>.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <param name="url">A URL representation the new destination of the redirect.</param>
        /// <returns>The redirect.</returns>
        public static T SetDestination<T>(this T redirect, string url) where T : IRedirect {

            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            
            return SetDestination(redirect, new RedirectDestination {
                Url = url,
                Type = RedirectDestinationType.Media
            });

        }

        /// <summary>
        /// Sets the destination of <paramref name="redirect"/> to the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <param name="content">A content or media item representing the new destination of the redirect.</param>
        /// <returns>The redirect.</returns>
        public static T SetDestination<T>(this T redirect, IPublishedContent content) where T : IRedirect {

            if (content == null) throw new ArgumentNullException(nameof(content));

            switch (content.ItemType) {

                case PublishedItemType.Content:
                    return SetDestination(redirect, new RedirectDestination {
                        Id = content.Id,
                        Key = content.Key,
                        Name = content.Name,
                        Url = content.Url(),
                        Type = RedirectDestinationType.Content
                    });

                case PublishedItemType.Media:
                    return SetDestination(redirect, new RedirectDestination {
                        Id = content.Id,
                        Key = content.Key,
                        Name = content.Name,
                        Url = content.Url(),
                        Type = RedirectDestinationType.Media
                    });

                default:
                    throw new RedirectsException($"Unsupported item type: {content.ItemType}");

            }

        }

        /// <summary>
        /// Sets the destination of <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <param name="destination">The destination.</param>
        /// <returns>The redirect.</returns>
        public static T SetDestination<T>(this T redirect, IRedirectDestination destination) where T : IRedirect {
            redirect.Destination = destination ?? throw new ArgumentNullException(nameof(destination));
            return redirect;

        }

    }

}