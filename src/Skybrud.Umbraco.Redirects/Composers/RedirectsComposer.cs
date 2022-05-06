using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.Redirects.ContentApps;
using Skybrud.Umbraco.Redirects.Factories.References;
using Skybrud.Umbraco.Redirects.Helpers;
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
            
            builder.Services.AddUnique<RedirectsServiceDependencies>();
            builder.Services.AddUnique<RedirectsBackOfficeHelperDependencies>();

            builder.Services.AddUnique<IRedirectsService, RedirectsService>();
            builder.Services.AddUnique<RedirectsBackOfficeHelper>();

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

        }

    }

}