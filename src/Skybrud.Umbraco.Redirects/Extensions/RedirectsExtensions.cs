using System;
using Microsoft.AspNetCore.Http;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

// ReSharper disable ConvertSwitchStatementToSwitchExpression
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace Skybrud.Umbraco.Redirects.Extensions {
    
    /// <summary>
    /// Static class with various extension methods used throughout the redirects package.
    /// </summary>
    public static class RedirectsExtensions {
        
        /// <summary>
        /// Returns the calculated destination URL for the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <param name="redirectsService">A reference to the current redirects service.</param>
        /// <returns>The destination URL.</returns>
        public static string GetDestinationUrl(this OutboundRedirect redirect, IRedirectsService redirectsService) {
            return redirectsService.GetDestinationUrl(redirect);
        }
        
        /// <summary>
        /// Returns the calculated destination URL for the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="redirect">The redirect.</param>
        /// <param name="uri">The inbound URL.</param>
        /// <param name="redirectsService">A reference to the current redirects service.</param>
        /// <returns>The destination URL.</returns>
        public static string GetDestinationUrl(this OutboundRedirect redirect, Uri uri, IRedirectsService redirectsService) {
            return redirectsService.GetDestinationUrl(redirect, uri);
        }
        
        /// <summary>
        /// Returns the <see cref="Uri"/> of the specified <paramref name="request"/>.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>An instance of <see cref="Uri"/>.</returns>
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
                Query = string.Empty,
                Fragment = string.Empty,
                Type = RedirectDestinationType.Url
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
                        Query = string.Empty,
                        Fragment = string.Empty,
                        Type = RedirectDestinationType.Content
                    });

                case PublishedItemType.Media:
                    return SetDestination(redirect, new RedirectDestination {
                        Id = content.Id,
                        Key = content.Key,
                        Name = content.Name,
                        Url = content.Url(),
                        Query = string.Empty,
                        Fragment = string.Empty,
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