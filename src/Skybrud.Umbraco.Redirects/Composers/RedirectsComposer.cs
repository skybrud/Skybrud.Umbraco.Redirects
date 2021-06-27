using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Skybrud.Umbraco.Redirects.Components;
using Skybrud.Umbraco.Redirects.Services;
using Skybrud.Umbraco.Redirects.Startup;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Composers {
    
    public class RedirectsComposer : IUserComposer {
        
        public void Compose(IUmbracoBuilder builder) {

            builder.Services.AddUnique<IRedirectsService, RedirectsService>();
            
            builder.Components().Append<MigrationComponent>();

            builder.Services.Insert(0,
                new ServiceDescriptor(typeof(IStartupFilter), _ => new RedirectsStartupFilter(),
                    ServiceLifetime.Transient));

        }

    }

}