using Microsoft.AspNetCore.Builder;

namespace Skybrud.Umbraco.Redirects.Middleware {
    
    public static class RedirectsBuilderExtensions {
        
        public static IApplicationBuilder UseRedirectsMiddleware(this IApplicationBuilder app) {
            return app.UseMiddleware<RedirectsMiddleware>();
        }

    }

}