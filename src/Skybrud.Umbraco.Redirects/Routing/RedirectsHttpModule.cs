using System;
using System.Web;
using Skybrud.LinkPicker;
using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Core.Models;
using Umbraco.Web;

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
            context.PostAuthorizeRequest += ContextOnPostAuthorizeRequest;
        }

        private void ContextOnPostAuthorizeRequest(object sender, EventArgs eventArgs) {


            // Look for a redirect matching the URL
            RedirectItem redirect = Repository.GetRedirectByUrl(Request.RawUrl);
            if (redirect == null) return;

            // Get the "IPublishedContent" of the node the redirect is pointing to (if not a raw URL)
            string redirectUrl = redirect.LinkUrl;
            //if (redirect.Link.Mode == LinkPickerMode.Content) {
            //    IPublishedContent content = UmbracoContext.Current.ContentCache.GetById(redirect.Link.Id);
            //    if (content != null) redirectUrl = content.Url;
            //} else if (redirect.Link.Mode == LinkPickerMode.Media) {
            //    IPublishedContent media = UmbracoContext.Current.MediaCache.GetById(redirect.Link.Id);
            //    if (media != null) redirectUrl = media.Url;
            //}

            // Redirect to the URL
            if (redirect.IsPermanent) {
                Response.RedirectPermanent(redirectUrl);
            } else {
                Response.Redirect(redirectUrl);
            }

        }

        public void Dispose() { }
    
    }

}
