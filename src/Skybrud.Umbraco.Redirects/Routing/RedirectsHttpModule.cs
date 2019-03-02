using System;
using System.Web;
using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Domains;
using Umbraco.Web.Composing;

namespace Skybrud.Umbraco.Redirects.Routing {

    /// <summary>
    /// HTTP module for handling inbound redirects.
    /// </summary>
    public class RedirectsHttpModule : IHttpModule {

        static Regex _capturingGroupsRegex = new Regex("\\$\\d+");

        /// <summary>
        /// Gets a reference to the current redirects repository.
        /// </summary>
		public RedirectsRepository Repository => RedirectsRepository.Current;

        /// <summary>
        /// Gets a reference to the current <see cref="HttpRequest"/>.
        /// </summary>
	    public HttpRequest Request => HttpContext.Current.Request;

        /// <summary>
        /// Gets a reference to the current <see cref="HttpResponse"/>.
        /// </summary>
        public HttpResponse Response => HttpContext.Current.Response;

        /// <summary>
        /// Initializes the HTTP module.
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context) {
            context.EndRequest += ContextOnEndRequest;
        }

        private Domain GetUmbracoDomain() {

            // Get the Umbraco request (it may be NULL)
            PublishedRequest pcr = Current.UmbracoContext == null ? null : Current.UmbracoContext.PublishedRequest;

            // Return the domain of the Umbraco request
            if (pcr != null) return pcr.Domain;

            // Find domain via DomainService based on current request domain
            var domain = DomainUtils.FindDomainForUri(Request.Url);

            if (domain != null) return domain;

            return null;

        }

        private void ContextOnEndRequest(object sender, EventArgs eventArgs) {

            HttpApplication application = (HttpApplication) sender;

            // Ignore if not a 404 response
            if (application.Response.StatusCode != 404) return;
            
            // Get the Umbraco domain of the current request
            Domain domain = GetUmbracoDomain();

            // Get the root node/content ID of the domain (no domain = 0)
            int rootNodeId = (domain == null ? 0 : domain.ContentId);

            // Look for a redirect matching the URL (and domain)
            RedirectItem redirect = null;
            if (rootNodeId > 0) redirect = Repository.GetRedirectByUrl(rootNodeId, Request.RawUrl);
            redirect = redirect ?? Repository.GetRedirectByUrl(0, Request.RawUrl);
            if (redirect == null) return;

			var redirectUrl = redirect.LinkUrl;

			if (redirect.ForwardQueryString) redirectUrl = Repository.HandleForwardQueryString(redirect, Request.RawUrl);

			//if (redirect.IsRegex)
			//{
			//    var regex = new Regex(redirect.Url);

			//    if (_capturingGroupsRegex.IsMatch(redirectUrl))
			//    {
			//        redirectUrl = regex.Replace(redirect.Url, redirectUrl);
			//    }
			//}

			// Redirect to the URL
			if (redirect.IsPermanent) {
                Response.RedirectPermanent(redirectUrl);
            } else {
                Response.Redirect(redirectUrl);
            }

        }

        /// <summary>
        /// Disposes the HTTP module.
        /// </summary>
        public void Dispose() { }
    
    }

}