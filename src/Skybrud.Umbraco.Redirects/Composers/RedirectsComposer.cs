using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.Redirects.Components;
using Skybrud.Umbraco.Redirects.Helpers;
using Skybrud.Umbraco.Redirects.Middleware;
using Skybrud.Umbraco.Redirects.Notifications.Handlers;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Composers {
    
    public class RedirectsComposer : IUserComposer {
        
        public void Compose(IUmbracoBuilder builder) {

            builder.Services.AddUnique<IRedirectsService, RedirectsService>();
            builder.Services.AddUnique<RedirectsBackOfficeHelper>();
            
            builder.Components().Append<MigrationComponent>();


            
            builder.AddNotificationHandler<ServerVariablesParsingNotification, ServerVariablesParsingHandler>();


            
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

        }

    }

}