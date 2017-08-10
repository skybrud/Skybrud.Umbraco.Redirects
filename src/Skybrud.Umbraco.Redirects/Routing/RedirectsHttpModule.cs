using System;
using System.Text;
using System.Web;
using Skybrud.Umbraco.Redirects.Models;
using umbraco.cms.businesslogic.web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Routing;
using Skybrud.Umbraco.Redirects.Domains;

namespace Skybrud.Umbraco.Redirects.Routing {

    public class RedirectsHttpModule : IHttpModule {

        public RedirectsRepository Repository {
            get { return RedirectsRepository.Current; }
        }

        public HttpRequest Request {
            get { return HttpContext.Current.Request; }
        }

        public HttpResponse Response {
            get { return HttpContext.Current.Response; }
        }

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

            // Redirect to the URL
            if (redirect.IsPermanent) {
                Response.RedirectPermanent(redirect.LinkUrl);
            } else {
                Response.Redirect(redirect.LinkUrl);
            }

        }

        public void Dispose() { }
    
    }

}