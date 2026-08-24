using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skybrud.Umbraco.Redirects.Middleware;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Skybrud.Umbraco.Redirects.Models.Settings;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Extensions;

/// <summary>
/// Static class with various extension methods used throughout the package.
/// </summary>
public static class RedirectsExtensions {

    private static readonly string[] _outboundPropertyAliases = ["outboundRedirect", "skyRedirect"];

    /// <summary>
    /// Registers a class used to configure the redirects settings. Note: These are run before all <see cref="PostConfigureRedirects"/>.
    /// </summary>
    /// <typeparam name="T">The class used for configuring the options.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/>> so that additional calls can be chained.</returns>
    public static IServiceCollection ConfigureRedirects<T>(this IServiceCollection services) where T : class, IConfigureOptions<RedirectsSettings> {
        return services.AddSingleton<IConfigureOptions<RedirectsSettings>, T>();
    }

    /// <summary>
    /// Registers an action used to configure the redirects settings. Note: These are run before all <see cref="PostConfigureRedirects"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">The action used to configure the options.</param>
    /// <returns>The <see cref="IServiceCollection"/>> so that additional calls can be chained.</returns>
    public static IServiceCollection ConfigureRedirects(this IServiceCollection services, Action<RedirectsSettings> configureOptions) {
        return services.Configure(configureOptions);
    }

    /// <summary>
    /// Registers a class used to configure the redirects settings. Note: These are run after all <see cref="ConfigureRedirects"/>.
    /// </summary>
    /// <typeparam name="T">The class used for configuring the options.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <returns>The <see cref="IServiceCollection"/>> so that additional calls can be chained.</returns>
    public static IServiceCollection PostConfigureRedirects<T>(this IServiceCollection services) where T : class, IPostConfigureOptions<RedirectsSettings> {
        return services.AddSingleton<IPostConfigureOptions<RedirectsSettings>, T>();
    }

    /// <summary>
    /// Registers an action used to configure the redirects settings. Note: These are run after all <see cref="ConfigureRedirects"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configureOptions">The action used to configure the options.</param>
    /// <returns>The <see cref="IServiceCollection"/>> so that additional calls can be chained.</returns>
    public static IServiceCollection PostConfigureRedirects(this IServiceCollection services, Action<RedirectsSettings> configureOptions) {
        return services.PostConfigure(configureOptions);
    }

    /// <summary>
    /// Returns the <see cref="Uri"/> of the specified <paramref name="request"/>.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>An instance of <see cref="Uri"/>.</returns>
    public static Uri GetUriForRedirects(this HttpRequest request) {

        // Initialize a new URI builder from the different parts of the specified request
        UriBuilder builder = new() {
            Scheme = request.Scheme,
            Host = request.Host.Host,
            Port = request.Host.Port ?? (request.Scheme == "https" ? 80 : 443),
            Path = request.Path,
            Query = request.QueryString.ToUriComponent()
        };

        // Should we update the URI?
        UpdateUriFromFeature(builder, request.HttpContext.Features.Get<IStatusCodeReExecuteFeature>());

        // Return the constructed URI
        return builder.Uri;

    }

    /// <see>
    ///     <cref>https://github.com/skybrud/Skybrud.Umbraco.Redirects/issues/181</cref>
    /// </see>
    /// <summary>
    /// ASP.NET Core supports using the <c>app.UseStatusCodePagesWithReExecute(...)</c> method during startup for
    /// setting a custom error page. When running the site via either <c>dotnet watch</c> or through Visual Studio,
    /// both <see cref="HttpRequest.Path"/> and <see cref="HttpRequest.QueryString"/> will still reflect the path
    /// and query string of the inbound request, but when running the site via <c>dotnet run</c> or hosting the
    /// site on a full IIS server, the values for the <see cref="HttpRequest.Path"/> and
    /// <see cref="HttpRequest.QueryString"/> properties will reflect the path and query string of the error page,
    /// causing the <see cref="RedirectsMiddleware"/> to look for redirects based on an incorrect URI.
    ///
    /// For the latter scenario, ASP.NET Core exposes a <see cref="IStatusCodeReExecuteFeature"/> that indicates
    /// the original path and query string, which we can then read and use accordingly.
    /// </summary>
    private static void UpdateUriFromFeature(UriBuilder builder, IStatusCodeReExecuteFeature? feature) {

        if (feature is null) return;
        builder.Path = feature.OriginalPath;
        builder.Query = feature.OriginalQueryString;

    }

    /// <param name="content">The content item holding the outbound redirect.</param>
    extension(IPublishedContent? content) {

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from either the
        /// <c>outboundRedirect</c> or <c>skyRedirect</c> properties.
        /// </summary>
        /// <returns>An instance of <see cref="IOutboundRedirect"/> if found; otherwise, <see langword="null"/>.</returns>
        public IOutboundRedirect? GetOutboundRedirect() {
            return content.GetOutboundRedirect(_outboundPropertyAliases);
        }

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from the property
        /// with specified alias <paramref name="propertyAlias"/>.
        /// </summary>
        /// <param name="propertyAlias">The alias of the property.</param>
        /// <returns>An instance of <see cref="IOutboundRedirect"/> if found; otherwise, <see langword="null"/>.</returns>
        public IOutboundRedirect? GetOutboundRedirect(string propertyAlias) {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyAlias);
            return content?.Value(propertyAlias) as IOutboundRedirect;
        }

        /// <summary>
        /// Returns an instance of <see cref="IOutboundRedirect"/> representing the outbound redirect from the first property
        /// matching the specified <paramref name="propertyAliases"/> where the value is an <see cref="IOutboundRedirect"/>.
        /// </summary>
        /// <param name="propertyAliases">The aliases of the properties.</param>
        /// <returns>An instance of <see cref="IOutboundRedirect"/> if found; otherwise, <see langword="null"/>.</returns>
        public IOutboundRedirect? GetOutboundRedirect(params string[] propertyAliases) {
            ArgumentNullException.ThrowIfNull(propertyAliases);
            foreach (string alias in propertyAliases) {
                if (content?.Value(alias) is IOutboundRedirect redirect) return redirect;
            }
            return null;
        }

        /// <summary>
        /// Attempts to get the outbound redirect for the current content.
        /// </summary>
        /// <param name="result">When the method returns <see langword="true"/>, contains the outbound redirect; otherwise, <see
        /// langword="null"/>.</param>
        /// <returns><see langword="true"/> if an outbound redirect was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetOutboundRedirect([NotNullWhen(true)] out IOutboundRedirect? result) {
            result = content.GetOutboundRedirect();
            return result is not null;
        }

        /// <summary>
        /// Attempts to get the outbound redirect associated with the specified property alias.
        /// </summary>
        /// <remarks>Throws <see cref="ArgumentException"/> if <paramref name="propertyAlias"/> is <see langword="null"/>, empty, or whitespace.</remarks>
        /// <param name="propertyAlias">The property alias to look up.</param>
        /// <param name="result">When this method returns <see langword="true"/>, contains the matching outbound redirect; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if an outbound redirect was found; otherwise, <see langword="false"/>.</returns>
        public bool TryGetOutboundRedirect(string propertyAlias, [NotNullWhen(true)] out IOutboundRedirect? result) {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyAlias);
            result = content.GetOutboundRedirect(propertyAlias);
            return result is not null;
        }

        /// <summary>
        /// Attempts to get an outbound redirect using the specified property aliases.
        /// </summary>
        /// <remarks>Throws <see cref="ArgumentNullException" /> when <paramref name="propertyAliases" />
        /// is <see langword="null" />.</remarks>
        /// <param name="propertyAliases">The property aliases to evaluate for an outbound redirect.</param>
        /// <param name="result">When this method returns <see langword="true" />, contains the outbound redirect; otherwise, <see
        /// langword="null" />.</param>
        /// <returns><see langword="true" /> if an outbound redirect is found; otherwise, <see langword="false" />.</returns>
        public bool TryGetOutboundRedirect(string[] propertyAliases, [NotNullWhen(true)] out IOutboundRedirect? result) {
            ArgumentNullException.ThrowIfNull(propertyAliases);
            result = content.GetOutboundRedirect(propertyAliases);
            return result is not null;
        }

    }

}