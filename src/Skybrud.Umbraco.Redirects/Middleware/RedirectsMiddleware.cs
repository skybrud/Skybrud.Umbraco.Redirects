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
                
                switch (context.Response.StatusCode) {
                    
                    case StatusCodes.Status404NotFound: {
                        
                        // Look for a redirect
                        IRedirect redirect = _redirectsService.GetRedirectByRequest(context.Request);
                    
                        // If a redirect is found, redirect the user to the destination URL
                        if (redirect != null) context.Response.Redirect(redirect.Destination.Url);
                        
                        break;

                    }

                }

                return Task.CompletedTask;

            });

            await _next(context);

        }

    }

}