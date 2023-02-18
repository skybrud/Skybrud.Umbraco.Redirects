using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Notifications;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Middleware {

    public class RedirectsMiddleware {

        private readonly RequestDelegate _next;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRuntimeState _runtimeState;
        private readonly IRedirectsService _redirectsService;

        public RedirectsMiddleware(RequestDelegate next, IEventAggregator eventAggregator, IRuntimeState runtimeState, IRedirectsService redirectsService) {
            _next = next;
            _eventAggregator = eventAggregator;
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

                        // Get the URI of the inbound request
                        Uri uri = context.Request.GetUri();

                        // Invoke the pre lookup event
                        RedirectPreLookupNotification preLookup = new(context);
                        _eventAggregator.Publish<IRedirectPreLookupNotification>(preLookup);

                        // Get the destination URL from the arguments (in case a value has been set
                        // from an notification handler)
                        string? destinationUrl = preLookup.DestinationUrl;

                        // Declare a variable for the redirect (either from the pre lookup or a lookup via the service)
                        IRedirect? redirect = preLookup.Redirect ?? _redirectsService.GetRedirectByRequest(context.Request);

                        // Return if we neither have a redirect or a destination URL
                        if (redirect == null && string.IsNullOrWhiteSpace(destinationUrl)) return Task.CompletedTask;

                        // Determine the redirect type
                        RedirectType redirectType = preLookup.RedirectType ?? redirect?.Type ?? RedirectType.Temporary;

                        // Calculate the destination URL
                        if (redirect is not null) destinationUrl ??= _redirectsService.GetDestinationUrl(redirect, uri);

                        // Invoke the post lookup event
                        RedirectPostLookupNotification postLookup = new(context, redirect, redirectType, destinationUrl);
                        _eventAggregator.Publish<IRedirectPostLookupNotification>(postLookup);

                        // Extract the values from the notification
                        redirectType = postLookup.RedirectType;
                        destinationUrl = postLookup.DestinationUrl;

                        // The destination URL should have a value at this point. If the value is empty, it's most
                        // likely because it was emptied via the post look up notification
                        if (string.IsNullOrWhiteSpace(destinationUrl)) return Task.CompletedTask;

                        // Respond with a redirect based on the redirect type
                        switch (redirectType) {

                            // If redirect is of type permanent, trigger a 301 redirect
                            case RedirectType.Permanent:
                                context.Response.Redirect(destinationUrl, true);
                                break;

                            // If redirect is of type temporary, trigger a 307 redirect
                            case RedirectType.Temporary:
                                context.Response.Redirect(destinationUrl, false, true);
                                break;

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