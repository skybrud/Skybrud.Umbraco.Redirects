using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Options;
using Skybrud.WebApi.Json;
using Skybrud.WebApi.Json.Meta;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Composing;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Skybrud.Umbraco.Redirects.Controllers.Api {
    
    /// <summary>
    /// WebAPI controller for managing redirects.
    /// </summary>
    [JsonOnlyConfiguration]
    [PluginController("Skybrud")]
    public class RedirectsController : UmbracoAuthorizedApiController {

        private CultureInfo _culture;
        private readonly IRedirectsService _redirects;

        #region Properties

        /// <summary>
        /// Gets a reference to the culture of the authenticated user.
        /// </summary>
        public CultureInfo Culture {
            // TODO: Is the language reliable for determining the culture?
            get { return _culture ?? (_culture = new CultureInfo(Security.CurrentUser.Language)); }
        }

        #endregion

        #region Constructors

        public RedirectsController(IRedirectsService redirectsService) {
            _redirects = redirectsService;
        }

        #endregion

        #region Public API methods

        /// <summary>
        /// Gets a list of all Umbraco domains.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetDomains() {
            RedirectDomain[] domains = _redirects.GetDomains();
            return new {
                total = domains.Length,
                items = domains
            };
        }

        /// <summary>
        /// Gets a list of root nodes based on the domains added to Umbraco. A root node will only be included in the
        /// list once - even if it has been assigned multiple domains.
        /// </summary>
        [HttpGet]
        public object GetRootNodes() {
            
            RedirectDomain[] domains = _redirects.GetDomains();

            List<RedirectRootNode> temp = new List<RedirectRootNode>();

            foreach (RedirectDomain domain in domains.Where(x => x.RootNodeId > 0).DistinctBy(x => x.RootNodeId)) {
                
                // Get the root node from the content service
                IContent content = Current.Services.ContentService.GetById(domain.RootNodeId);
                
                // Skip if not found via the content service
                if (content == null) continue;
                
                // Skip if the root node is located in the recycle bin
                if (content.Path.StartsWith("-1,-20,")) continue;
                
                // Append the root node to the result
                temp.Add(RedirectRootNode.GetFromContent(content));
            
            }

            return new {
                total = temp.Count,
                items = temp.OrderBy(x => x.Id)
            };
        
        }
        
        /// <summary>
        /// Gets a paginated list of all redirects.
        /// </summary>
        /// <param name="page">The page to be returned.</param>
        /// <param name="limit">The maximum amount of redirects to be returned per page.</param>
        /// <param name="type">The type of redirects that should be returned.</param>
        /// <param name="text">The text that the returned redirects should match.</param>
        /// <param name="rootNodeId">The root node ID that the returned redirects should match. <c>null</c> means all redirects. <c>0</c> means all global redirects.</param>
        /// <returns>A list of redirects.</returns>
        [HttpGet]
        public object GetRedirects(int page = 1, int limit = 20, string type = null, string text = null, int? rootNodeId = null) {
            try {
                return _redirects.GetRedirects(page, limit, type, text, rootNodeId);
            } catch (RedirectsException ex) {
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            }
        }

        /// <summary>
        /// Gets a list of all redirects for the content item matching the specified <paramref name="contentId"/>.
        /// </summary>
        /// <param name="contentId">The ID of the content item.</param>
        /// <returns>A list of redirects.</returns>
        [HttpGet]
        public object GetRedirectsForContent(int contentId) {

            try {
                
                // Get a reference to the content item
                IContent content = Current.Services.ContentService.GetById(contentId);

                // Trigger an exception if the content item couldn't be found
                if (content == null) throw new RedirectsException(HttpStatusCode.NotFound, Localize("redirects/errorContentNoRedirects"));
                
                // Generate the response
                return JsonMetaResponse.GetSuccess(new {
                    content = new {
                        id = content.Id,
                        name = content.Name
                    },
                    redirects = _redirects.GetRedirectsByContentId(contentId)
                });
            
            } catch (RedirectsException ex) {
                
                // Generate the error response
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            
            }
        
        }

        /// <summary>
        /// Gets a list of all redirects for the media item matching the specified <paramref name="contentId"/>.
        /// </summary>
        /// <param name="contentId">The ID of the media item.</param>
        /// <returns>A list of redirects.</returns>
        [HttpGet]
        public object GetRedirectsForMedia(int contentId) {

            try {

                // Get a reference to the media item
                IMedia media = Current.Services.MediaService.GetById(contentId);

                // Trigger an exception if the media item couldn't be found
                if (media == null) throw new RedirectsException(HttpStatusCode.NotFound, Localize("redirects/errorMediaNoRedirects"));

                // Generate the response
                return JsonMetaResponse.GetSuccess(new {
                    media = new {
                        id = media.Id,
                        name = media.Name
                    },
                    redirects = _redirects.GetRedirectsByMediaId(contentId)
                });
            
            } catch (RedirectsException ex) {

                // Generate the error response
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            
            }

        }

        [HttpPost]
        public object AddRedirect([FromBody] JObject m) {

            var model = m.ToObject<AddRedirectOptions>();

            try {
                
                // Some input validation
                if (string.IsNullOrWhiteSpace(model.OriginalUrl)) throw new RedirectsException(Localize("redirects/errorNoUrl") + "----");
                if (string.IsNullOrWhiteSpace(model.Destination?.Url)) throw new RedirectsException(Localize("redirects/errorNoDestination") + "\r\n\r\n" + m);

                // Add the redirect
                RedirectItem redirect = _redirects.AddRedirect(model);

                // Return the redirect
                return redirect;

            } catch (RedirectsException ex) {

                // Generate the error response
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            
            }

        }

        /// <summary>
        /// Adds a new redirect.
        /// </summary>
        /// <param name="rootNodeId">The root node ID. <c>0</c> indicates a global redirect.</param>
        /// <param name="url">The inbound URL of the redirect.</param>
        /// <param name="linkMode">The mode/type of the destination link.</param>
        /// <param name="linkId">The media or content ID of the destination link.</param>
        /// <param name="linkKey">The media or content key of the destination link.</param>
        /// <param name="linkUrl">The URL of the destination link.</param>
        /// <param name="permanent">Indicates whether the redirect should be permanent. Default is <c>true</c>.</param>
        /// <param name="regex">Indicates wether the inbound URL is a REGEX pattern. <c>false</c> by default.</param>
        /// <param name="forward">Indicates whether the query string should be forwarded. <c>false</c> by default.</param>
        /// <returns>The created redirect.</returns>
        [HttpGet]
        public object AddRedirect(int rootNodeId, string url, string linkMode, int linkId, Guid linkKey, string linkUrl, bool permanent = true, bool regex = false, bool forward = false) {

            try {
                
                // Some input validation
                if (string.IsNullOrWhiteSpace(url)) throw new RedirectsException(Localize("redirects/errorNoUrl"));
                if (string.IsNullOrWhiteSpace(linkUrl)) throw new RedirectsException(Localize("redirects/errorNoDestination"));
                if (string.IsNullOrWhiteSpace(linkMode)) throw new RedirectsException(Localize("redirects/errorNoDestination"));

                // Parse the link mode
                RedirectDestinationType mode;
                switch (linkMode) {
                    case "content": mode = RedirectDestinationType.Content; break;
                    case "media": mode = RedirectDestinationType.Media; break;
                    case "url": mode = RedirectDestinationType.Url; break;
                    default: throw new RedirectsException(Localize("redirects/errorUnknownLinkMode"));
                }

                // Initialize a new link item
                RedirectDestination destination = new RedirectDestination(linkId, linkKey, linkUrl, mode);

                // Add the redirect
                RedirectItem redirect = _redirects.AddRedirect(rootNodeId, url, destination, permanent, regex, forward);

                // Return the redirect
                return redirect;

            } catch (RedirectsException ex) {

                // Generate the error response
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            
            }

        }

        [HttpPost]
        public object EditRedirect(Guid redirectId, [FromBody] EditRedirectOptions model) {
            
            try {
                
                // Get a reference to the redirect
                RedirectItem redirect = _redirects.GetRedirectByKey(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                // Some input validation
                if (string.IsNullOrWhiteSpace(model.OriginalUrl)) throw new RedirectsException(Localize("redirects/errorNoUrl"));
                if (string.IsNullOrWhiteSpace(model.Destination?.Url)) throw new RedirectsException(Localize("redirects/errorNoDestination"));

                // Split the URL and query string
                string[] urlParts = model.OriginalUrl.Split('?');
                string url = urlParts[0].TrimEnd('/');
                string query = urlParts.Length == 2 ? urlParts[1] : string.Empty;
				
                redirect.RootId = model.RootNodeId;
                redirect.RootKey = model.RootNodeKey;
                redirect.Url = url;
                redirect.QueryString = query;
                redirect.LinkId = model.Destination.Id;
                redirect.LinkKey = model.Destination.Key;
                redirect.LinkUrl = model.Destination.Url;
                redirect.LinkMode = model.Destination.Type;
                redirect.IsPermanent = model.IsPermanent;
                redirect.ForwardQueryString = model.ForwardQueryString;

                // Save/update the redirect
                _redirects.SaveRedirect(redirect);

                // Return the redirect
                return redirect;

            } catch (RedirectsException ex) {

                // Generate the error response
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            
            }

        }

        /// <summary>
        /// Edits the redirect with the specified <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="rootNodeId">The root node ID. <c>0</c> indicates a global redirect.</param>
        /// <param name="redirectId">The ID of the redirect.</param>
        /// <param name="url">The inbound URL of the redirect.</param>
        /// <param name="linkMode">The mode/type of the destination link.</param>
        /// <param name="linkId">The media or content ID of the destination link.</param>
        /// <param name="linkKey">The media or content key of the destination link.</param>
        /// <param name="linkUrl">The URL of the destination link.</param>
        /// <param name="permanent">Indicates whether the redirect should be permanent. Default is <c>true</c>.</param>
        /// <param name="regex">Indicates wether the inbound URL is a REGEX pattern. <c>false</c> by default.</param>
        /// <param name="forward">Indicates whether the query string should be forwarded. <c>false</c> by default.</param>
        /// <returns>The updated redirect.</returns>
        [HttpGet]
        public object EditRedirect(int rootNodeId, Guid redirectId, string url, string linkMode, int linkId, Guid linkKey, string linkUrl, bool permanent = true, bool regex = false, bool forward = false) {
            
            try {

                // Get a reference to the redirect
                RedirectItem redirect = _redirects.GetRedirectByKey(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                // Some input validation
                if (string.IsNullOrWhiteSpace(url)) throw new RedirectsException(Localize("redirects/errorNoUrl"));
                if (string.IsNullOrWhiteSpace(linkUrl)) throw new RedirectsException(Localize("redirects/errorNoDestination"));
                if (string.IsNullOrWhiteSpace(linkMode)) throw new RedirectsException(Localize("redirects/errorNoDestination"));

                // Parse the link mode
                RedirectDestinationType mode;
                switch (linkMode) {
                    case "content": mode = RedirectDestinationType.Content; break;
                    case "media": mode = RedirectDestinationType.Media; break;
                    case "url": mode = RedirectDestinationType.Url; break;
                    default: throw new RedirectsException(Localize("redirects/errorUnknownLinkMode"));
                }

                // Initialize a new link item
                RedirectDestination destination = new RedirectDestination(linkId, linkKey, linkUrl, mode);

                // Split the URL and query string
                string[] urlParts = url.Split('?');
                url = urlParts[0].TrimEnd('/');
                string query = urlParts.Length == 2 ? urlParts[1] : string.Empty;

                // Update the properties of the redirect
                redirect.RootId = rootNodeId;
                redirect.Url = url;
                redirect.QueryString = query;
                redirect.Link = destination;
                redirect.IsPermanent = permanent;
				redirect.IsRegex = regex;
                redirect.ForwardQueryString = forward;

                // Save/update the redirect
                _redirects.SaveRedirect(redirect);

                // Return the redirect
                return redirect;

            } catch (RedirectsException ex) {

                // Generate the error response
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            
            }

        }

        /// <summary>
        /// Deletes the redirect with the specified <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="redirectId">The ID of the redirect.</param>
        [HttpGet]
        public object DeleteRedirect(Guid redirectId) {

            try {

                // Get a reference to the redirect
                RedirectItem redirect = _redirects.GetRedirectByKey(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                // Delete the redirect
                _redirects.DeleteRedirect(redirect);

                // Return the redirect
                return redirect;

            } catch (RedirectsException ex) {

                // Generate the error response
                return Request.CreateResponse(JsonMetaResponse.GetError(HttpStatusCode.InternalServerError, ex.Message));
            
            }

        }

        #endregion

        #region Private helper methods

        private string Localize(string key) {
            return Services.TextService.Localize(key, Culture);
        }

        #endregion

    }

}