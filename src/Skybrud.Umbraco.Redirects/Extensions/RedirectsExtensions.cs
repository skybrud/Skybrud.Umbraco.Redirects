using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable ConvertSwitchStatementToSwitchExpression
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace Skybrud.Umbraco.Redirects.Extensions {

    /// <summary>
    /// Static class with various extension methods used throughout the redirects package.
    /// </summary>
    public static class RedirectsExtensions {

        private static readonly string[] _outboundPropertyAliases = { "skyRedirect", "outboundRedirect" };

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
                        Name = content.Name!,
                        Url = content.Url(),
                        Query = string.Empty,
                        Fragment = string.Empty,
                        Type = RedirectDestinationType.Content
                    });

                case PublishedItemType.Media:
                    return SetDestination(redirect, new RedirectDestination {
                        Id = content.Id,
                        Key = content.Key,
                        Name = content.Name!,
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

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from either the
        /// <c>skyRedirect</c> or <c>outboundRedirect</c> properties.
        ///
        /// This method will ensure that you'll always get an instance of <see cref="IOutboundRedirect"/> even though
        /// the property may not hold a redirect. You can then use the <see cref="IOutboundRedirect.IsValid"/> property
        /// to check that the redirect is in fact valid.
        /// </summary>
        /// <param name="content">The content item holding the outbound rediderect.</param>
        /// <returns>An instance of <see cref="IOutboundRedirect"/>.</returns>
        public static IOutboundRedirect GetOutboundRedirect(this IPublishedContent content) {
            return GetOutboundRedirect(content, _outboundPropertyAliases);
        }

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from the property
        /// with specified alias <paramref name="propertyAlias"/>.
        ///
        /// This method will ensure that you'll always get an instance of <see cref="IOutboundRedirect"/> even though
        /// the property may not hold a redirect. You can then use the <see cref="IOutboundRedirect.IsValid"/> property
        /// to check that the redirect is in fact valid.
        /// </summary>
        /// <param name="content">The content item holding the outbound rediderect.</param>
        /// <param name="propertyAlias">The alias of the property.</param>
        /// <returns>An instance of <see cref="IOutboundRedirect"/>.</returns>
        public static IOutboundRedirect GetOutboundRedirect(this IPublishedContent content, string propertyAlias) {
            if (string.IsNullOrWhiteSpace(propertyAlias)) throw new ArgumentNullException(nameof(propertyAlias));
            IOutboundRedirect? redirect = content.Value(propertyAlias) as IOutboundRedirect;
            return redirect ?? new OutboundRedirect();
        }

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from the first property
        /// matching the specified <paramref name="propertyAliases"/> where the value is an <see cref="IOutboundRedirect"/>.
        ///
        /// This method will ensure that you'll always get an instance of <see cref="IOutboundRedirect"/> even though
        /// the property may not hold a redirect. You can then use the <see cref="IOutboundRedirect.IsValid"/> property
        /// to check that the redirect is in fact valid.
        /// </summary>
        /// <param name="content">The content item holding the outbound rediderect.</param>
        /// <param name="propertyAliases">The aliases of the properties.</param>
        /// <returns>An instance of <see cref="IOutboundRedirect"/>.</returns>
        public static IOutboundRedirect GetOutboundRedirect(this IPublishedContent content, params string[] propertyAliases) {

            if (propertyAliases == null) throw new ArgumentNullException(nameof(propertyAliases));

            foreach (string alias in propertyAliases) {
                if (content.Value(alias) is IOutboundRedirect redirect) return redirect;
            }

            return new OutboundRedirect();

        }

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from either the
        /// <c>skyRedirect</c> or <c>outboundRedirect</c> properties.
        ///
        /// If the property doesn't hold a <see cref="IOutboundRedirect"/> value, <see langword="null"/> will be returned instead.
        /// </summary>
        /// <param name="content">The content item holding the outbound rediderect.</param>
        /// <returns>An instance of <see cref="IOutboundRedirect"/> if successful; otherwise, <see langword="null"/>.</returns>
        public static IOutboundRedirect? GetOutboundRedirectOrDefault(this IPublishedContent content) {
            return GetOutboundRedirectOrDefault(content, _outboundPropertyAliases);
        }

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from the property
        /// with specified alias <paramref name="propertyAlias"/>.
        ///
        /// If the property doesn't hold a <see cref="IOutboundRedirect"/> value, <see langword="null"/> will be returned instead.
        /// </summary>
        /// <param name="content">The content item holding the outbound rediderect.</param>
        /// <param name="propertyAlias">The alias of the property.</param>
        /// <returns>An instance of <see cref="IOutboundRedirect"/> if successful; otherwise, <see langword="null"/>.</returns>
        public static IOutboundRedirect? GetOutboundRedirectOrDefault(this IPublishedContent content, string propertyAlias) {
            if (string.IsNullOrWhiteSpace(propertyAlias)) throw new ArgumentNullException(nameof(propertyAlias));
            return content.Value(propertyAlias) as IOutboundRedirect;
        }

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from the first property
        /// matching the specified <paramref name="propertyAliases"/> where the value is an <see cref="IOutboundRedirect"/>.
        ///
        /// If the property doesn't hold a <see cref="IOutboundRedirect"/> value, <see langword="null"/> will be returned instead.
        /// </summary>
        /// <param name="content">The content item holding the outbound rediderect.</param>
        /// <param name="propertyAliases">The aliases of the properties.</param>
        /// <returns>An instance of <see cref="IOutboundRedirect"/> if successful; otherwise, <see langword="null"/>.</returns>
        public static IOutboundRedirect? GetOutboundRedirectOrDefault(this IPublishedContent content, params string[] propertyAliases) {

            if (propertyAliases == null) throw new ArgumentNullException(nameof(propertyAliases));

            foreach (string alias in propertyAliases) {
                if (content.Value(alias) is IOutboundRedirect redirect) return redirect;
            }

            return null;

        }

        #region HttpContext

        /// <summary>
        /// Attempts to redirect the user if a matching redirect exists for the requested URL.
        /// </summary>
        /// <param name="context">The HTTP context.</param>
        /// <returns><see langword="true"/> if a redirect is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryRedirectUser(this HttpContext context)  {
            return TryRedirectUser(context.Response);
        }

        #endregion

        #region HttpResponse

        /// <summary>
        /// Attempts to redirect the user if a matching redirect exists for the requested URL.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns><see langword="true"/> if a redirect is found; otherwise, <see langword="false"/>.</returns>
        public static bool TryRedirectUser(this HttpResponse response) {

            // Get a reference to the redirects service
            IRedirectsService redirectsService = response.HttpContext.RequestServices.GetRequiredService<IRedirectsService>();

            // Get the URI of the current request
            Uri uri = response.HttpContext.Request.GetUri();

            // Query the redirects service for a matching redirect
            IRedirect? redirect = redirectsService.GetRedirectByUri(uri);
            if (redirect?.Destination?.Url is null) return false;

            // Redirect the user
            response.Redirect(redirect, uri);

            return true;

        }

        /// <summary>
        /// Redirects the user to the destination of the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="redirect">The redirect.</param>
        public static void Redirect(this HttpResponse response, IRedirect redirect) {
            Redirect(response, redirect, response.HttpContext.Request.GetUri());
        }

        /// <summary>
        /// Redirects the user to the destination of the specified <paramref name="redirect"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="redirect">The redirect.</param>
        /// <param name="uri">The current URI.</param>
        public static void Redirect(this HttpResponse response, IRedirect redirect, Uri uri) {

            // Get a reference to the redirects service
            IRedirectsService redirectsService = response.HttpContext.RequestServices.GetRequiredService<IRedirectsService>();

            // Get the destination URL of the redirect
            string destinationUrl = redirectsService.GetDestinationUrl(redirect, uri);

            // Redirect the user
            Redirect(response, destinationUrl, redirect.Type);

        }

        /// <summary>
        /// Redirects the user to the specified <paramref name="destinationUrl"/>.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <param name="destinationUrl">The URL to which the user should be redirected.</param>
        /// <param name="redirectType">The type of the redirect - either <see cref="RedirectType.Permanent"/>
        /// (<c>301</c>) or <see cref="RedirectType.Temporary"/> (<c>307</c>).</param>
        public static void Redirect(this HttpResponse response, string destinationUrl, RedirectType redirectType) {

            // Respond with a redirect based on the redirect type
            switch (redirectType) {

                // If redirect is of type permanent, trigger a 301 redirect
                case RedirectType.Permanent:
                    response.Redirect(destinationUrl, true);
                    break;

                // If redirect is of type temporary, trigger a 307 redirect
                case RedirectType.Temporary:
                    response.Redirect(destinationUrl, false, true);
                    break;

                // Ideally we should have hit the default case, as all options should be handled above. If not, we
                // create a new temporary redirect (307)
                default:
                    response.Redirect(destinationUrl, false, true);
                    break;

            }

        }

        #endregion

    }

}