using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.Redirects.Components;
using Skybrud.Umbraco.Redirects.Middleware;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Composers {
    
    public class RedirectsComposer : IUserComposer {
        
        public void Compose(IUmbracoBuilder builder) {

            builder.Services.AddUnique<IRedirectsService, RedirectsService>();
            
            builder.Components().Append<MigrationComponent>();
            
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