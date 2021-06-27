using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Services;

namespace Skybrud.Umbraco.Redirects.Middleware {

    public class RedirectsMiddleware {

        readonly RequestDelegate _next;
        private readonly IRedirectsService _redirectsService;

        public RedirectsMiddleware(RequestDelegate next, IRedirectsService redirectsService) {
            _next = next;
            _redirectsService = redirectsService;
        }

        public async Task Invoke(HttpContext context) {

            Redirect redirect = _redirectsService.GetRedirectByRequest(context.Request);

            context.Response.Redirect(redirect.Destination.Url);

            await _next(context);

        }

    }

}