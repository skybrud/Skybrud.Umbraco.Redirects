using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Skybrud.Umbraco.Redirects.Config;
using Skybrud.Umbraco.Redirects.Middleware;

namespace Skybrud.Umbraco.Redirects.Extensions;

/// <summary>
/// Static class with various extension methods used throughout the package.
/// </summary>
public static class RedirectsExtensions {

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

}