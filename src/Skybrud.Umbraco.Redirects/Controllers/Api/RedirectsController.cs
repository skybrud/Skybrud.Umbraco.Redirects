using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Enums;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Helpers;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Api;
using Skybrud.Umbraco.Redirects.Models.Options;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco.Extensions;

#pragma warning disable 1591

// ReSharper disable IntroduceOptionalParameters.Local

namespace Skybrud.Umbraco.Redirects.Controllers.Api {

    /// <summary>
    /// API controller for managing redirects.
    /// </summary>
    [PluginController("Skybrud")]
    public class RedirectsController : UmbracoAuthorizedApiController {

        private readonly ILogger<RedirectsController> _logger;
        private readonly IRedirectsService _redirects;
        private readonly RedirectsBackOfficeHelper _backOffice;
        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        #region Constructors

        public RedirectsController(ILogger<RedirectsController> logger, IRedirectsService redirectsService, RedirectsBackOfficeHelper backOffice,
            IContentService contentService,
            IMediaService mediaService,
            IUmbracoContextAccessor umbracoContextAccessor) {
            _logger = logger;
            _redirects = redirectsService;
            _backOffice = backOffice;
            _contentService = contentService;
            _mediaService = mediaService;
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        #endregion

        /// <summary>
        /// Gets a list of root nodes based on the domains added to Umbraco. A root node will only be included in the
        /// list once - even if it has been assigned multiple domains.
        /// </summary>
        [HttpGet]
        public ActionResult GetRootNodes() {

	        var rootNodes = _backOffice.Settings.ContentApp.UseStartNodes
		        ? _redirects.GetRootNodes(_backOffice.CurrentUser)
		        : _redirects.GetRootNodes();

	        return new JsonResult(new
	        {
		        total = rootNodes.Length,
		        items = rootNodes.Select(x => new RedirectRootNodeModel(x, _backOffice))
	        });
        }

        [HttpPost]
        public ActionResult AddRedirect([FromBody] JObject m) {

            AddRedirectOptions model = m.ToObject<AddRedirectOptions>();

            try {

                // Some input validation
                if (model == null) throw new RedirectsException("Failed parsing request body.");
                if (string.IsNullOrWhiteSpace(model.OriginalUrl)) throw new RedirectsException(_backOffice.Localize("errorNoUrl"));
                if (string.IsNullOrWhiteSpace(model.Destination?.Url)) throw new RedirectsException(_backOffice.Localize("errorNoDestination"));

                // Add the redirect
                IRedirect redirect = _redirects.AddRedirect(model);

                // Map the result for the API
                return new JsonResult(_backOffice.Map(redirect));

            } catch (RedirectsException ex) {

                if (!ex.Is404) _logger.LogError(ex, ex.Message);

                // Generate the error response
                return Error(ex);

            }

        }

        [HttpPost]
        public ActionResult EditRedirect(Guid redirectId, [FromBody] EditRedirectOptions model) {

            try {

                // Get a reference to the redirect
                IRedirect redirect = _redirects.GetRedirectByKey(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                // Some input validation
                if (model == null) throw new RedirectsException("Failed parsing request body.");
                if (string.IsNullOrWhiteSpace(model.OriginalUrl)) throw new RedirectsException(_backOffice.Localize("errorNoUrl"));
                if (string.IsNullOrWhiteSpace(model.Destination?.Url)) throw new RedirectsException(_backOffice.Localize("errorNoDestination"));

                // Split the URL and query string
                string[] urlParts = model.OriginalUrl.Split('?');
                string url = urlParts[0].TrimEnd('/');
                string query = urlParts.Length == 2 ? urlParts[1] : string.Empty;

                redirect.RootKey = model.RootNodeKey;
                redirect.Url = url;
                redirect.QueryString = query;
                redirect.Destination = model.Destination;
                redirect.IsPermanent = model.IsPermanent;
                redirect.ForwardQueryString = model.ForwardQueryString;

                // Save/update the redirect
                _redirects.SaveRedirect(redirect);

                // Map the result for the API
                return new JsonResult(_backOffice.Map(redirect));

            } catch (RedirectsException ex) {

                if (!ex.Is404) _logger.LogError(ex, ex.Message);

                // Generate the error response
                return Error(ex);

            }

        }

        /// <summary>
        /// Edits the redirect with the specified <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="rootNodeKey">The root node key. <see cref="Guid.Empty"/> indicates a global redirect.</param>
        /// <param name="redirectId">The ID of the redirect.</param>
        /// <param name="url">The inbound URL of the redirect.</param>
        /// <param name="linkMode">The mode/type of the destination link.</param>
        /// <param name="linkId">The media or content ID of the destination link.</param>
        /// <param name="linkKey">The media or content key of the destination link.</param>
        /// <param name="linkUrl">The URL of the destination link.</param>
        /// <param name="permanent">Indicates whether the redirect should be permanent. Default is <c>true</c>.</param>
        /// <param name="forward">Indicates whether the query string should be forwarded. <c>false</c> by default.</param>
        /// <returns>The updated redirect.</returns>
        [HttpGet]
        public ActionResult EditRedirect(Guid rootNodeKey, Guid redirectId, string url,
            string linkMode, int linkId, Guid linkKey, string linkUrl,
            bool permanent = true, bool forward = false) {

            try {

                // Get a reference to the redirect
                IRedirect redirect = _redirects.GetRedirectByKey(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                // Some input validation
                if (string.IsNullOrWhiteSpace(url)) throw new RedirectsException(_backOffice.Localize("errorNoUrl"));
                if (string.IsNullOrWhiteSpace(linkUrl)) throw new RedirectsException(_backOffice.Localize("errorNoDestination"));
                if (string.IsNullOrWhiteSpace(linkMode)) throw new RedirectsException(_backOffice.Localize("errorNoDestination"));

                // Parse the destination type
                RedirectDestinationType type;
                switch (linkMode) {
                    case "content": type = RedirectDestinationType.Content; break;
                    case "media": type = RedirectDestinationType.Media; break;
                    case "url": type = RedirectDestinationType.Url; break;
                    default: throw new RedirectsException(_backOffice.Localize("errorUnknownLinkType"));
                }

                // Initialize a new destination instance
                RedirectDestination destination = new RedirectDestination {
                    Id = linkId,
                    Key = linkKey,
                    Type = type,
                    Name = redirect.Destination?.Name
                };

                // Split the URL and query string
                string[] urlParts = url.Split('?');
                url = urlParts[0].TrimEnd('/');
                string query = urlParts.Length == 2 ? urlParts[1] : string.Empty;

                // Update the properties of the redirect
                redirect.RootKey = rootNodeKey;
                redirect.Url = url;
                redirect.QueryString = query;
                redirect.SetDestination(destination);
                redirect.IsPermanent = permanent;
                redirect.ForwardQueryString = forward;

                // Save/update the redirect
                _redirects.SaveRedirect(redirect);

                // Map the result for the API
                return new JsonResult(_backOffice.Map(redirect));

            } catch (RedirectsException ex) {

                if (!ex.Is404) _logger.LogError(ex, ex.Message);

                // Generate the error response
                return Error(ex);

            }

        }

        /// <summary>
        /// Deletes the redirect with the specified <paramref name="redirectId"/>.
        /// </summary>
        /// <param name="redirectId">The ID of the redirect.</param>
        [HttpGet]
        public ActionResult DeleteRedirect(Guid redirectId) {

            try {

                // Get a reference to the redirect
                IRedirect redirect = _redirects.GetRedirectByKey(redirectId);
                if (redirect == null) throw new RedirectNotFoundException();

                // Delete the redirect
                _redirects.DeleteRedirect(redirect);

                // Map the result for the API
                return new JsonResult(_backOffice.Map(redirect));

            } catch (RedirectsException ex) {

                if (!ex.Is404) _logger.LogError(ex, ex.Message);

                // Generate the error response
                return Error(ex);

            }

        }

        /// <summary>
        /// Gets a paginated list of all redirects.
        /// </summary>
        /// <param name="page">The page to be returned.</param>
        /// <param name="limit">The maximum amount of redirects to be returned per page.</param>
        /// <param name="type">The type of redirects that should be returned.</param>
        /// <param name="text">The text that the returned redirects should match.</param>
        /// <param name="rootNodeKeys">An array of root node keys that the returned redirects should match. <c>null</c> means all redirects. <see cref="Guid.Empty"/> means all global redirects.</param>
        /// <returns>A list of redirects.</returns>
        [HttpGet]
        public ActionResult GetRedirects(int page = 1, int limit = 20, string type = null, string text = null, [FromQuery]Guid[] rootNodeKeys = null) {

            try {
	            var rootKeys = _backOffice.Settings.ContentApp.UseStartNodes
		            ? _redirects.GetRootNodes(_backOffice.CurrentUser).Select(p => p.Key)
		            : _redirects.GetRootNodes().Select(p => p.Key);

                // Initialize the search options
                RedirectsSearchOptions options = new() {
                    Page = page,
                    Limit = limit,
                    Type = EnumUtils.ParseEnum(type, RedirectTypeFilter.All),
                    Text = text,
                    RootNodeKeys = (rootNodeKeys != null && rootNodeKeys.Any()) ? rootNodeKeys : rootKeys.ToArray()
                };

                // Make the search for redirects via the redirects service
                RedirectsSearchResult result = _redirects.GetRedirects(options);

                // Map the result for the API
                return new JsonResult(_backOffice.Map(result));

            } catch (RedirectsException ex) {

                if (!ex.Is404) _logger.LogError(ex, ex.Message);

                // Generate the error response
                return Error(ex);

            }

        }

        [HttpGet]
        public ActionResult GetRedirectsForNode(string type, Guid key) {

            try {

                RedirectNodeModel node;

                switch (type) {

                    case "content":

                        // Get a reference to the content item
                        IContent content1 = _contentService.GetById(key);

                        // Trigger an exception if the content item couldn't be found
                        if (content1 == null) throw new RedirectsException(HttpStatusCode.NotFound, _backOffice.Localize("errorContentNoRedirects"));

                        // Look up the content via the content cahce
                        IPublishedContent content2 = null;
                        if (_umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext umbraco)) {
                            content2 = umbraco.Content.GetById(key);
                        }

                        // Initialize a new model instance
                        node = new RedirectNodeModel(content1, content2);

                        break;

                    case "media":

                        // Get a reference to the media item
                        IMedia media1 = _mediaService.GetById(key);

                        // Trigger an exception if the media item couldn't be found
                        if (media1 == null) throw new RedirectsException(HttpStatusCode.NotFound, _backOffice.Localize("errorContentNoRedirects"));

                        // Look up the media via the content cahce
                        IPublishedContent media2 = null;
                        if (_umbracoContextAccessor.TryGetUmbracoContext(out umbraco)) {
                            media2 = umbraco.Content.GetById(key);
                        }

                        // Initialize a new model instance
                        node = new RedirectNodeModel(media1, media2);

                        break;

                    default:
                        throw new RedirectsException(HttpStatusCode.BadRequest, $"Unsupported node type: {type}");


                }

                // get the redirects via the redirects service
                IRedirect[] redirects = _redirects.GetRedirectsByNodeKey(node.Type, key);

                // Generate the response
                return new JsonResult(new {
                    node,
                    redirects = _backOffice.Map(redirects)
                });

            } catch (RedirectsException ex) {

                // Generate the error response
                return Error(ex);

            }

        }

        private ActionResult Error(RedirectsException ex) {

            // Initialize a new error model based on the exception
            RedirectsError body = new RedirectsError(ex);

            return new JsonResult(body) {
                StatusCode = (int) ex.StatusCode
            };

        }
    }

}