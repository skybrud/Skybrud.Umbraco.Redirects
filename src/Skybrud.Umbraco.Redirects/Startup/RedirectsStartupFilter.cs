using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Skybrud.Umbraco.Redirects.Middleware;

namespace Skybrud.Umbraco.Redirects.Startup {
    
    public class RedirectsStartupFilter : IStartupFilter {
        
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next) => app => {
            app.UseRedirectsMiddleware();
            next(app);
        };

    }

}