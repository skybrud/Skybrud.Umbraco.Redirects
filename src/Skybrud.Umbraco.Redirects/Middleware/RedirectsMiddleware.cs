using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Services;

namespace Skybrud.Umbraco.Redirects.Middleware {
    
    public class RedirectsMiddleware {
        
        private readonly RequestDelegate _next;
        private readonly IRedirectsService _redirectsService;

        public RedirectsMiddleware(RequestDelegate next, IRedirectsService redirectsService) {
            _next = next;
            _redirectsService = redirectsService;
        }

        public async Task InvokeAsync(HttpContext context) {

            string pathAndQuery = context.Request.GetEncodedPathAndQuery();

            // Ignore all /umbraco/ requests
            if (pathAndQuery.IndexOf("/umbraco/", StringComparison.InvariantCultureIgnoreCase) == 0)  {
                await _next(context);
                return;
            }

            context.Response.OnStarting(() => {
                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    // Look for a redirect
                    Redirect redirect = _redirectsService.GetRedirectByRequest(context.Request);

                    if (redirect != null)
                    {
                        // Redirect the user to the destination URL
                        context.Response.Redirect(redirect.Destination.Url);
                    }
                }

                return Task.CompletedTask;
            });

            await _next(context);
        }

    }

}