﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace Skybrud.Umbraco.Redirects.Middleware {
    
    public class RedirectsMiddleware {
        
        private readonly RequestDelegate _next;
        private readonly IRuntimeState _runtimeState;
        private readonly IRedirectsService _redirectsService;
        private readonly IUmbracoContextFactory _umbracoContextFactory;

        public RedirectsMiddleware(RequestDelegate next, IRuntimeState runtimeState, IRedirectsService redirectsService, IUmbracoContextFactory umbracoContextFactory) {
            _next = next;
            _runtimeState = runtimeState;
            _redirectsService = redirectsService;
            _umbracoContextFactory = umbracoContextFactory;
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
                        
                        // Ensure there is an Umbraco context available
                        using (_ = _umbracoContextFactory.EnsureUmbracoContext())
                        {
                            // Look for a redirect
                            IRedirect redirect = _redirectsService.GetRedirectByRequest(context.Request);

                            // Respond with a redirect based on the redirect type
                            switch (redirect?.Type) {

                                // If redirect is of type permanent, trigger a 301 redirect
                                case RedirectType.Permanent:
                                    context.Response.Redirect(redirect.Destination.Url, true);
                                    break;

                                // If redirect is of type temporary, trigger a 307 redirect
                                case RedirectType.Temporary:
                                    context.Response.Redirect(redirect.Destination.Url, false, true);
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
