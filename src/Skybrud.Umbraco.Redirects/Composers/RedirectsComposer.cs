using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Config;
using Skybrud.Umbraco.Redirects.ContentApps;
using Skybrud.Umbraco.Redirects.Dashboards;
using Skybrud.Umbraco.Redirects.Factories;
using Skybrud.Umbraco.Redirects.Factories.References;
using Skybrud.Umbraco.Redirects.Helpers;
using Skybrud.Umbraco.Redirects.Manifests;
using Skybrud.Umbraco.Redirects.Middleware;
using Skybrud.Umbraco.Redirects.Notifications.Handlers;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Composers {

    public class RedirectsComposer : IComposer {

        public void Compose(IUmbracoBuilder builder) {

            builder.Services.AddOptions<RedirectsSettings>()
                .Bind(builder.Config.GetSection("Skybrud:Redirects"), o => o.BindNonPublicProperties = true)
                .ValidateDataAnnotations();

            builder.Services.AddSingleton<RedirectsServiceDependencies>();
            builder.Services.AddSingleton<RedirectsBackOfficeHelperDependencies>();

            builder.Services.AddUnique<IRedirectsService, RedirectsService>();
            builder.Services.AddSingleton<RedirectsBackOfficeHelper>();

            builder.Services.AddSingleton<RedirectsModelsFactory>();

            builder.ManifestFilters().Append<RedirectsManifestFilter>();

            builder.ContentApps()?.Append<RedirectsContentAppFactory>();

            builder.AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesParsingHandler>();
            builder.AddNotificationHandler<UmbracoApplicationStartingNotification, UmbracoApplicationStartingHandler>();

            builder.Services.Configure<UmbracoPipelineOptions>(options => {
                options.AddFilter(new UmbracoPipelineFilter(
                    "SkybrudRedirects",
                    _ => { },
                    applicationBuilder => {
                        applicationBuilder.UseMiddleware<RedirectsMiddleware>();
                    },
                    _ => { }
                ));
            });

            builder.DataValueReferenceFactories()?.Append<OutboundRedirectReferenceFactory>();

            ConfigureDashboard(builder);

        }

        private static void ConfigureDashboard(IUmbracoBuilder builder) {

            // The dashboard should be enabled by default, but if explicitly set to "false", we should remove it
            bool dashboardEnabled = builder.Config
                .GetSection("Skybrud:Redirects:Dashboard:Enabled").Value
                .ToBoolean(true);

            if (!dashboardEnabled) builder.Dashboards().Remove<RedirectsDashboard>();

        }

    }

}