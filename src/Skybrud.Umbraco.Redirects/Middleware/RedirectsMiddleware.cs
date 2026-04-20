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
using Umbraco.Cms.Core.Web;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Middleware;

public class RedirectsMiddleware {

    private readonly RequestDelegate _next;
    private readonly IEventAggregator _eventAggregator;
    private readonly IRuntimeState _runtimeState;
    private readonly IRedirectsService _redirectsService;
    private readonly IUmbracoContextFactory _umbracoContextFactory;

    public RedirectsMiddleware(RequestDelegate next, IEventAggregator eventAggregator, IRuntimeState runtimeState, IRedirectsService redirectsService, IUmbracoContextFactory umbracoContextFactory) {
        _next = next;
        _eventAggregator = eventAggregator;
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

        // Check for redirects proactively BEFORE the request continues to Umbraco.
        // This ensures redirects work even when Umbraco finds and renders a 404 page.
        if (TryHandleRedirect(context)) {
            return; // Redirect was performed, stop the pipeline
        }

        await _next(context);

    }

    /// <summary>
    /// Attempts to handle a redirect for the current request proactively.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns><c>true</c> if a redirect was performed; otherwise, <c>false</c>.</returns>
    private bool TryHandleRedirect(HttpContext context) {

        // Get the URI of the inbound request
        Uri uri = context.Request.GetUriForRedirects();

        // Make sure we have an Umbraco context (we need it for various lookups)
        using UmbracoContextReference reference = _umbracoContextFactory.EnsureUmbracoContext();

        // Invoke the pre lookup event
        RedirectPreLookupNotification preLookup = new(context);
        _eventAggregator.Publish<IRedirectPreLookupNotification>(preLookup);

        // Return if the notification has been marked as canceled
        if (preLookup.Cancel) return false;

        // Get the destination URL from the arguments
        string? destinationUrl = preLookup.DestinationUrl;

        // Look for a redirect
        IRedirect? redirect;
        if (preLookup.Redirect is not null) {
            redirect = preLookup.Redirect;
        } else if (!string.IsNullOrWhiteSpace(destinationUrl)) {
            redirect = null;
        } else {
            redirect = _redirectsService.GetRedirectByRequest(context.Request);
        }

        // Return if we neither have a redirect nor a destination URL
        if (redirect == null && string.IsNullOrWhiteSpace(destinationUrl)) return false;

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

        // Return if destination URL was cleared
        if (string.IsNullOrWhiteSpace(destinationUrl)) return false;

        // Perform the redirect
        switch (redirectType) {
            case RedirectType.Permanent:
                context.Response.Redirect(destinationUrl, true);
                break;
            case RedirectType.Temporary:
                context.Response.Redirect(destinationUrl, false, true);
                break;
        }

        return true;
    }

}