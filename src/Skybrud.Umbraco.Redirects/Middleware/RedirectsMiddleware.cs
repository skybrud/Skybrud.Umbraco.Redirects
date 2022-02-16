using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;

namespace Skybrud.Umbraco.Redirects.Middleware {
    
    public class RedirectsMiddleware {
        
        private readonly RequestDelegate _next;
        private readonly IRuntimeState _runtimeState;
        private readonly IRedirectsService _redirectsService;

        public RedirectsMiddleware(RequestDelegate next, IRuntimeState runtimeState, IRedirectsService redirectsService) {
            _next = next;
            _runtimeState = runtimeState;
            _redirectsService = redirectsService;
        }

        public async Task InvokeAsync(HttpContext context) {

            // If Umbraco hasn't been installed yet, the middleware shouldn't do anything (interacting with the
            // redirects service will fail as the database isn't setup yet)
            if (_runtimeState.Level == RuntimeLevel.Install) {
                await _next(context);
                return;
            }

            string pathAndQuery = context.Request.GetEncodedPathAndQuery();

            // Ignore all /umbraco/ requests
            if (pathAndQuery.IndexOf("/umbraco/", StringComparison.InvariantCultureIgnoreCase) == 0) {
                await _next(context);
                return;
            }

            context.Response.OnStarting(() => {
                
                switch (context.Response.StatusCode) {
                    
                    case StatusCodes.Status404NotFound: {
                        
                        // Look for a redirect
                        IRedirect redirect = _redirectsService.GetRedirectByRequest(context.Request);

                        if (redirect != null) {

                            string destinationUrl = _redirectsService.GetDestinationUrl(redirect, context.Request.GetUri().PathAndQuery);

                            // Respond with a redirect based on the redirect type
                            switch (redirect.Type) {

                                // If redirect is of type permanent, trigger a 301 redirect
                                case RedirectType.Permanent:
                                    context.Response.Redirect(destinationUrl, true);
                                    break;

                                // If redirect is of type temporary, trigger a 307 redirect
                                case RedirectType.Temporary:
                                    context.Response.Redirect(destinationUrl, false, true);
                                    break;

                            }
                        }

                        break;

                    }

                }

                return Task.CompletedTask;

            });

            await _next(context);

        }

    }

}