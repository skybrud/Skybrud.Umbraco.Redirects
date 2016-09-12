using System.Web;
using Skybrud.LinkPicker;
using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace Skybrud.Umbraco.Redirects.Routing {

    public class RedirectsContentFinder : IContentFinder {

        #region Properties

        public HttpRequest Request {
            get { return HttpContext.Current.Request; }
        }

        public RedirectsRepository Repository { get; private set; }

        #endregion

        #region Constructors

        public RedirectsContentFinder() {
            Repository = new RedirectsRepository();
        }

        #endregion

        public bool TryFindContent(PublishedContentRequest request) {

            // Look for a redirect matching the URL
            RedirectItem redirect = Repository.GetRedirectByUrl(Request.RawUrl);
            if (redirect == null) return false;

            // Get the "IPublishedContent" of the node the redirect is pointing to (if not a raw URL)
            string redirectUrl = redirect.LinkUrl;
            if (redirect.Link.Mode == LinkPickerMode.Content) {
                IPublishedContent content = UmbracoContext.Current.ContentCache.GetById(redirect.Link.Id);
                if (content != null) redirectUrl = content.Url;
            } else if (redirect.Link.Mode == LinkPickerMode.Media) {
                IPublishedContent media = UmbracoContext.Current.MediaCache.GetById(redirect.Link.Id);
                if (media != null) redirectUrl = media.Url;
            }

            // Redirect to the URL
            if (redirect.IsPermanent) {
                request.SetRedirectPermanent(redirectUrl);
            } else {
                request.SetRedirect(redirectUrl);
            }

            // We did succeed, but also have a redirect
            return true;

        }
    
    }

}