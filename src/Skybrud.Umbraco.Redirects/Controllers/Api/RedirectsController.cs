using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core;
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

        /// <summary>
        /// Gets a list of root nodes based on the domains added to Umbraco. A root node will only be included in the
        /// list once - even if it has been assigned multiple domains.
        /// </summary>
        [HttpGet]
        public object GetRootNodes() {
            
            RedirectDomain[] domains = Repository.GetDomains();

            List<RedirectRootNode> temp = new List<RedirectRootNode>();

            foreach (RedirectDomain domain in domains.Where(x => x.RootNodeId > 0).DistinctBy(x => x.RootNodeId)) {
                
                // Get the root node from the content service
                IContent content = ApplicationContext.Services.ContentService.GetById(domain.RootNodeId);
                
                // Skip if not found via the content service
                if (content == null) continue;
                
                // Skip if the root node is located in the recycle bin
                if (content.Path.StartsWith("-1,-20,")) continue;
                
                // Append the root node to the result
                temp.Add(RedirectRootNode.GetFromContent(content));
            
            }

            return new {
                total = temp.Count,
                data = temp.OrderBy(x => x.Id)
            };
        
        }
        
        [HttpGet]
        public object GetRedirects(int page = 1, int limit = 20, string type = null, string text = null, int? rootNodeId = null) {
            try {
                return Repository.GetRedirects(page, limit, type, text, rootNodeId);
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
                    redirects = Repository.GetRedirectsByContentId(contentId)
                });
            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }
        
        }

        [HttpGet]
        public object GetRedirectsForMedia(int contentId) {

            IMedia media = ApplicationContext.Services.MediaService.GetById(contentId);
            if (media == null) throw new RedirectsException(HttpStatusCode.NotFound, "A media item with the specified ID could not be found.");

            try {
                return JsonMetaResponse.GetSuccess(new {
                    media = new {
                        id = media.Id,
                        name = media.Name
                    },
                    redirects = Repository.GetRedirectsByMediaId(contentId)
                });
            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }

        }

        [HttpGet]
        public object AddRedirect(int rootNodeId, string url, string linkMode, int linkId, string linkUrl, string linkName = null, bool permanent = true, bool regex = false) {

            try {

                RedirectLinkItem redirect = RedirectLinkItem.Parse(new JObject {
                    {"id", linkId},
                    {"name", linkName + ""},
                    {"url", linkUrl},
                    {"mode", linkMode}
                });

                return Repository.AddRedirect(rootNodeId, url, redirect, permanent, regex);

            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }

        }

        [HttpGet]
        public object EditRedirect(int rootNodeId, string redirectId, string url, string linkMode, int linkId, string linkUrl, string linkName = null, bool permanent = true, bool regex = false) {

            try {

                // Get the redirect from the database
                RedirectItem redirect = Repository.GetRedirectById(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                if (String.IsNullOrWhiteSpace(url)) throw new RedirectsException("You must specify a URL for the redirect.");
                if (String.IsNullOrWhiteSpace(linkUrl)) throw new RedirectsException("You must specify a destination link for the redirect.");
                if (String.IsNullOrWhiteSpace(linkMode)) throw new RedirectsException("You must specify a destination link for the redirect.");

                // Initialize a new link picker item
                RedirectLinkItem link = RedirectLinkItem.Parse(new JObject {
                    {"id", linkId},
                    {"name", linkName + ""},
                    {"url", linkUrl},
                    {"mode", linkMode}
				});

                string[] urlParts = url.Split('?');
                url = urlParts[0].TrimEnd('/');
                string query = urlParts.Length == 2 ? urlParts[1] : "";

                redirect.RootNodeId = rootNodeId;
                redirect.Url = url;
                redirect.QueryString = query;
                redirect.Link = link;
                redirect.IsPermanent = permanent;
				redirect.IsRegex = regex;
                
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