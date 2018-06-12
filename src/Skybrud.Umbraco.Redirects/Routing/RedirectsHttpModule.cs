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

        private IDomain GetUmbracoDomain() {

            // Get the Umbraco request (it may be NULL)
            PublishedContentRequest pcr = UmbracoContext.Current == null ? null : UmbracoContext.Current.PublishedContentRequest;

            // Return the domain of the Umbraco request
            if (pcr != null) return pcr.UmbracoDomain;

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
            IDomain domain = GetUmbracoDomain();

            // Get the root node/content ID of the domain (no domain = 0)
            int rootNodeId = (domain == null || domain.RootContentId == null ? 0 : domain.RootContentId.Value);

            // Look for a redirect matching the URL (and domain)
            RedirectItem redirect = null;
            if (rootNodeId > 0) redirect = Repository.GetRedirectByUrl(rootNodeId, Request.RawUrl);
            redirect = redirect ?? Repository.GetRedirectByUrl(0, Request.RawUrl);
            if (redirect == null) return;

			var redirectUrl = redirect.LinkUrl;
            
            if (redirect.ForwardQueryString) {
                
                Uri redirectUri = new Uri(redirectUrl.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ? redirectUrl : string.Format("{0}{1}{2}{3}/{4}", Request.Url.Scheme, Uri.SchemeDelimiter, Request.Url.Host, Request.Url.Port != 80 ? string.Concat(":", Request.Url.Port) : string.Empty, redirectUrl.StartsWith("/") ? redirectUrl.Substring(1) : redirectUrl));

                NameValueCollection redirectQueryString = HttpUtility.ParseQueryString(redirectUri.Query);
                NameValueCollection newQueryString = HttpUtility.ParseQueryString(Request.Url.Query);

                if (redirectQueryString.HasKeys()) {
                    newQueryString = newQueryString.Merge(redirectQueryString);
                }
                string pathAndQuery = Uri.UnescapeDataString(redirectUri.PathAndQuery) + redirectUri.Fragment;
                redirectUri = new Uri(string.Format("{0}{1}{2}{3}/{4}{5}", redirectUri.Scheme, Uri.SchemeDelimiter, redirectUri.Host, redirectUri.Port != 80 ? string.Concat(":", redirectUri.Port) : string.Empty, pathAndQuery.Contains("?") ? pathAndQuery.Substring(0, pathAndQuery.IndexOf('?')) : pathAndQuery.StartsWith("/") ? pathAndQuery.Substring(1) : pathAndQuery, newQueryString.HasKeys() ? string.Concat("?", newQueryString.ToQueryString()) : string.Empty));

                redirectUrl = redirectUri.AbsoluteUri;
            }

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