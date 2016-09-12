using System;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Skybrud.LinkPicker;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core.Models;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Redirects.Controllers.Api {
    
    [JsonOnlyConfiguration]
    public class RedirectsController : UmbracoAuthorizedApiController {

        protected RedirectsRepository Repository = new RedirectsRepository();

        [HttpGet]
        public object GetDomains() {
            RedirectDomain[] domains = Repository.GetDomains();
            return new {
                total = domains.Length,
                data = domains
            };
        }

        [HttpGet]
        public object GetRedirects() {

            try {
                return JsonMetaResponse.GetSuccess(Repository.GetRedirects());
            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }

        }

        [HttpGet]
        public object GetRedirectsForContent(int contentId) {

            IContent content = ApplicationContext.Services.ContentService.GetById(contentId);
            if (content == null) throw new RedirectsException(HttpStatusCode.NotFound, "A content item with the specified ID could not be found.");
 
            try {
                return JsonMetaResponse.GetSuccess(new {
                    content = new {
                        id = content.Id,
                        name = content.Name
                    },
                    redirects = Repository.GetRedirectsForContents(contentId)
                });
            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }
        
        }

        [HttpGet]
        public object AddRedirect(string url, string linkMode, int linkId, string linkUrl, string linkName = null) {

            try {

                LinkPickerItem redirect = LinkPickerItem.Parse(new JObject {
                    {"id", linkId},
                    {"name", linkName + ""},
                    {"url", linkUrl},
                    {"mode", linkMode}
                });

                return Repository.AddRedirect(url, redirect);

            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }

        }

        [HttpGet]
        public object EditRedirect(string redirectId, string url, string linkMode, int linkId, string linkUrl, string linkName = null) {

            try {

                // Get the redirect from the database
                RedirectItem redirect = Repository.GetRedirectById(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                if (String.IsNullOrWhiteSpace(url)) throw new RedirectsException("You must specify a URL for the redirect.");
                if (String.IsNullOrWhiteSpace(linkUrl)) throw new RedirectsException("You must specify a destination link for the redirect.");
                if (String.IsNullOrWhiteSpace(linkMode)) throw new RedirectsException("You must specify a destination link for the redirect.");

                // Initialize a new link picker item
                LinkPickerItem link = LinkPickerItem.Parse(new JObject {
                    {"id", linkId},
                    {"name", linkName + ""},
                    {"url", linkUrl},
                    {"mode", linkMode}
                });

                string[] urlParts = url.Split('?');
                url = urlParts[0].TrimEnd('/');
                string query = urlParts.Length == 2 ? urlParts[1] : "";

                redirect.Url = url;
                redirect.QueryString = query;
                redirect.Link = link;
                
                Repository.SaveRedirect(redirect);

                return redirect;

            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }

        }

        [HttpGet]
        public object DeleteRedirect(string redirectId) {

            try {

                // Get the redirect from the database
                RedirectItem redirect = Repository.GetRedirectById(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                Repository.DeleteRedirect(redirect);

                return redirect;

            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }

        }

    }

}