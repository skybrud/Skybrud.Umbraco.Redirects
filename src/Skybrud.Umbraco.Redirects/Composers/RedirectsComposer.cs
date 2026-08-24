using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.Redirects.Api;
using Skybrud.Umbraco.Redirects.Config;
using Skybrud.Umbraco.Redirects.Factories;
using Skybrud.Umbraco.Redirects.Helpers;
using Skybrud.Umbraco.Redirects.Manifests;
using Skybrud.Umbraco.Redirects.Middleware;
using Skybrud.Umbraco.Redirects.Notifications.Handlers;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Infrastructure.Manifest;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Composers;

public class RedirectsComposer : IComposer {

    public void Compose(IUmbracoBuilder builder) {

        // Register the appsettings configuration section
        builder.Services.AddOptions<RedirectsSettings>()
            .Bind(builder.Config.GetSection("Skybrud:Redirects"), o => o.BindNonPublicProperties = true)
            .ValidateDataAnnotations();

        builder.Services.AddSingleton<RedirectsServiceDependencies>();
        builder.Services.AddSingleton<RedirectsBackOfficeHelperDependencies>();

        builder.Services.AddSingleton<IRedirectsService, RedirectsService>();
        builder.Services.AddSingleton<RedirectsBackOfficeHelper>();
        builder.Services.AddSingleton<RedirectsModelFactory>();

        builder.AddNotificationAsyncHandler<UmbracoApplicationStartingNotification, UmbracoApplicationStartingHandler>();

        builder.Services.AddSingleton<IPackageManifestReader, RedirectsPackageManifestReader>();

        builder.Services.Configure<UmbracoPipelineOptions>(options => {
            options.AddFilter(new UmbracoPipelineFilter(
                "SkybrudRedirects",
                _ => { },
                _ => { },
                applicationBuilder => {
                    applicationBuilder.UseMiddleware<RedirectsMiddleware>();
                },
                _ => { }
            ));
        });

        builder.Services.ConfigureOptions<RedirectsSwaggerGenOptions>();

    }

}