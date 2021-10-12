using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;

namespace Skybrud.Umbraco.Redirects.Middleware {
    
    public class RedirectsMiddleware {
        
        private readonly RequestDelegate _next;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IRedirectsService _redirectsService;

        public RedirectsMiddleware(RequestDelegate next, IUmbracoContextAccessor umbracoContextAccessor, IRedirectsService redirectsService) {
            _next = next;
            _umbracoContextAccessor = umbracoContextAccessor;
            _redirectsService = redirectsService;
        }

        public async Task InvokeAsync(HttpContext context) {

            string pathAndQuery = context.Request.GetEncodedPathAndQuery();

            // Ignore all /umbraco/ requests
            if (pathAndQuery.IndexOf("/umbraco/", StringComparison.InvariantCultureIgnoreCase) == 0)  {
                await _next(context);
                return;
            }

            if (ShouldIgnoreRequest())
            {
                await _next(context);
                return;
            }

            // Look for a redirect
            Redirect redirect = _redirectsService.GetRedirectByRequest(context.Request);

            // Carry on like normal if no matching redirects are found
            if (redirect == null) {
                await _next(context);
                return;
            }
            
            // Redirect the user to the destination URL
            context.Response.Redirect(redirect.Destination.Url);

        }

        private bool ShouldIgnoreRequest()
        {
            // Get the current Umbraco context
            if (!_umbracoContextAccessor.TryGetUmbracoContext(out var umbracoContext))
            {
                return false;
            }

            // Get a reference to the published request (may be null)
            var publishedRequest = umbracoContext?.PublishedRequest;

            // If we're inside the Umbraco request pipeline, and an IPublishedContent has been set,
            // we don't do anything except a 404 status is set
            // 404 is set, when Umbraco found a custom 404 page
            // if no redirect is set up, custom 404 page will still be shown ( see redirect equals null condition above)
            return publishedRequest?.ResponseStatusCode != 404 && publishedRequest?.HasPublishedContent() == true;
        }

    }

}