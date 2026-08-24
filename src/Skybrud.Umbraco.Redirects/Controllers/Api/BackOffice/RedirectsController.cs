using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Collections.Enumerables.Extensions;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Security.Extensions;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Api;
using Skybrud.Umbraco.Redirects.Config;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Helpers;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Api;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Routing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Extensions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Controllers.Api.BackOffice;

[ApiController]
[VersionedApiBackOfficeRoute(RedirectsApiConstants.Route)]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[MapToApi(RedirectsApiConstants.Alias)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = RedirectsApiConstants.GroupName)]
public class RedirectsController : Controller {

    private readonly ILogger<RedirectsController> _logger;
    private readonly IOptions<RedirectsSettings> _settings;
    private readonly IContentService _contentService;
    private readonly ILocalizedTextService _localizedTextService;
    private readonly IRedirectsService _redirectsService;
    private readonly RedirectsBackOfficeHelper _backOfficeHelper;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IDocumentUrlService _documentUrlService;

    #region Constructors

    public RedirectsController(ILogger<RedirectsController> logger, IOptions<RedirectsSettings> settings, IContentService contentService, ILocalizedTextService localizedTextService, IRedirectsService redirectsService, RedirectsBackOfficeHelper backOfficeHelper, IUmbracoContextAccessor umbracoContextAccessor, IDocumentUrlService documentUrlService) {
        _logger = logger;
        _settings = settings;
        _contentService = contentService;
        _localizedTextService = localizedTextService;
        _redirectsService = redirectsService;
        _backOfficeHelper = backOfficeHelper;
        _umbracoContextAccessor = umbracoContextAccessor;
        _documentUrlService = documentUrlService;
    }

    #endregion

    #region Public API methods

    /// <summary>
    /// Gets a paginated list of all redirects.
    /// </summary>
    /// <param name="page">The page to be returned.</param>
    /// <param name="limit">The maximum amount of redirects to be returned per page.</param>
    /// <param name="rootNodeKey">The root node key that the returned redirects should match. <c>null</c> means all redirects. <see cref="Guid.Empty"/> means all global redirects.</param>
    /// <param name="type">A comma separated list of redirect types that should be returned.</param>
    /// <param name="text">The text that the returned redirects should match.</param>
    /// <returns>A list of redirects.</returns>
    [HttpGet]
    [EndpointSummary("Returns a paginated list of redirects.")]
    [EndpointDescription("Returns a paginated list of redirects based on the specified search options.")]
    public RedirectSearchResultModel Index(int page = 1, int limit = 20, Guid? rootNodeKey = null, string? type = null, string? text = null) {

        // Initialize the search options
        RedirectsSearchOptions options = new() {
            Page = page,
            Limit = limit,
            RootNodeKey = rootNodeKey,
            Type = EnumUtils.ParseEnumList<RedirectDestinationType>(type),
            Text = text
        };

        // Make the search for redirects via the redirects service
        RedirectsSearchResult result = _redirectsService.GetRedirects(options);

        // Map to API models
        return _backOfficeHelper.Map(result);

    }

    [HttpGet("{type}/{key}")]
    [EndpointSummary("Returns a list of redirects for a specific node.")]
    [EndpointDescription("Returns the redirects for the specified node type and key.")]
    public ActionResult<RedirectListModel> GetRedirects(string type, Guid key) {

        // Parse the node type
        if (!EnumUtils.TryParseEnum(type, out RedirectDestinationType nodeType) || nodeType == RedirectDestinationType.Url) {
            return BadRequest($"Invalid node type '{type}'.");
        }

        // Get the redirects for the specified node type and key
        IReadOnlyList<IRedirect> redirects = _redirectsService.GetRedirectsByNodeKey(nodeType, key);

        // Map the result for the API
        return _backOfficeHelper.Map(redirects);

    }

    [HttpPut("")]
    [EndpointSummary("Adds a new redirect.")]
    [EndpointDescription("Adds a new redirect with the specified options.")]
    public ActionResult<RedirectItem> AddRedirect([FromBody] AddRedirectOptions options) {

        try {

            // Some input validation
            if (string.IsNullOrWhiteSpace(options.OriginalUrl)) throw new RedirectsException(_backOfficeHelper.Localize("errorNoUrl"));
            if (string.IsNullOrWhiteSpace(options.Destination.Url)) throw new RedirectsException(_backOfficeHelper.Localize("errorNoDestination"));

            // Add the redirect
            IRedirect redirect = _redirectsService.AddRedirect(options);

            // Currently the UI only supports entering the destination URL, so we need to check whether it matches an
            // existing content or media item, if so, overwrite the destination to reflect this
            TryUpdateDestination(redirect);

            // Map the result for the API
            return Ok(_backOfficeHelper.Map(redirect));

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed adding redirect.");

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Returns a redirect.")]
    [EndpointDescription("Returns the redirect with the specified integer ID.")]
    public ActionResult<RedirectItem> GetRedirect(int id) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectById(id) ?? throw new RedirectNotFoundException(id);

            // Map the result for the API
            RedirectItem result = _backOfficeHelper.Map(redirect);

            // Return the result
            return Ok(result);

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed getting redirect with ID '{Id}'.", id);

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpGet("{key:guid}")]
    [EndpointSummary("Returns a redirect.")]
    [EndpointDescription("Returns the redirect with the specified GUID key.")]
    public ActionResult<RedirectItem> GetRedirect(Guid key) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectByKey(key) ?? throw new RedirectNotFoundException(key);

            // Map the result for the API
            RedirectItem result = _backOfficeHelper.Map(redirect);

            // Return the result
            return Ok(result);

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed getting redirect with GUID key '{Key}'.", key);

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpPatch("{key:guid}")]
    [EndpointSummary("Updates a redirect.")]
    [EndpointDescription("Updates the redirect with the specified GUID key.")]
    public ActionResult<RedirectItem> EditRedirect(Guid key, [FromBody] EditRedirectOptions options) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectByKey(key) ?? throw new RedirectNotFoundException(key);

            // Some input validation
            if (string.IsNullOrWhiteSpace(options.OriginalUrl)) throw new RedirectsException(_backOfficeHelper.Localize("errorNoUrl"));
            if (string.IsNullOrWhiteSpace(options.Destination.Url)) throw new RedirectsException(_backOfficeHelper.Localize("errorNoDestination"));

            // Split the URL (path) and query string
            options.OriginalUrl.Split('?', out string url, out string? query);

            // Update the redirect with the updated values
            redirect.RootKey = options.RootNodeKey;
            redirect.Url = url.TrimEnd('/');
            redirect.QueryString = query;
            redirect.Destination = options.Destination;
            redirect.IsPermanent = options.IsPermanent;
            redirect.ForwardQueryString = options.ForwardQueryString;

            // Currently the UI only supports entering the destination URL, so we need to check whether it matches an
            // existing content or media item, if so, overwrite the destination to reflect this
            TryUpdateDestination(redirect);

            // Save/update the redirect
            _redirectsService.SaveRedirect(redirect);

            // Map the result for the API
            RedirectItem result = _backOfficeHelper.Map(redirect);

            // Return the result
            return Ok(result);

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed updating redirect with GUID key '{Key}'.", key);

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Deletes a redirect")]
    [EndpointDescription("Deletes the redirect with the specified ID.")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Description = "No Content - The redirect was successfully deleted.")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Not Found - The redirect with the specified ID was not found.")]
    public IActionResult DeleteRedirect(int id) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectById(id) ?? throw new RedirectNotFoundException(id);

            // Delete the redirect
            _redirectsService.DeleteRedirect(redirect);

            // Return a 204 No Content response to indicate successful deletion
            return NoContent();

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed deleting redirect with ID '{Id}'.", id);

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpDelete("{key:guid}")]
    [EndpointSummary("Deletes a redirect")]
    [EndpointDescription("Deletes the redirect with the specified GUID key.")]
    [ProducesResponseType(StatusCodes.Status204NoContent, Description = "No Content - The redirect was successfully deleted.")]
    [ProducesResponseType(StatusCodes.Status404NotFound, Description = "Not Found - The redirect with the specified GUID key was not found.")]
    public IActionResult DeleteRedirect(Guid key) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectByKey(key) ?? throw new RedirectNotFoundException(key);

            // Delete the redirect
            _redirectsService.DeleteRedirect(redirect);

            // Return a 204 No Content response to indicate successful deletion
            return NoContent();

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed deleting redirect with GUID key '{Key}'.", key);

            // Generate the error response
            return Error(ex);

        }

    }

    /// <summary>
    /// Returns a list of root nodes based on the domains added to Umbraco. A root node will only be included in the
    /// list once - even if it has been assigned multiple domains.
    /// </summary>
    [HttpGet("rootNodes")]
    [EndpointSummary("Returns a list of root nodes.")]
    [EndpointDescription("Returns a list of root nodes based on the domains added to Umbraco. A root node will only be included in the list once - even if it has been assigned multiple domains.")]
    [ProducesResponseType(StatusCodes.Status200OK, Description = "OK - The list of root nodes was successfully retrieved.")]
    public ActionResult<RedirectsRootNodeListModel> GetRootNodes() {

        IReadOnlyList<RedirectRootNode> rootNodes = _redirectsService.GetRootNodes();

        RedirectsRootNodeListModel result = _backOfficeHelper.MapRootNodes(rootNodes);

        return result;

    }

    /// <summary>
    /// Returns a list of cultures for the content node with the specified <paramref name="key"/>. If the node does not vary by culture, an empty list will be returned instead.
    /// </summary>
    /// <param name="key">The GUID key of the content node.</param>
    /// <returns>A list of cultures.</returns>
    [HttpGet("content/{key:guid}/cultures")]
    [EndpointSummary("Returns a list of cultures for the content node with the specified GUID key.")]
    [EndpointDescription("Returns a list of cultures for the content node with the specified GUID key. If the node does not vary by culture, an empty list will be returned instead.")]
    public async Task<ActionResult<IReadOnlyList<RedirectsCultureModel>>> GetCultures(Guid key) {

        // We start by looking for a published version of the content, as this is the fastest
        // approach, and still should expose the information that we need
        if (_backOfficeHelper.TryGetContent(key, out IPublishedContent? content)) {
            return Ok(await _backOfficeHelper.GetCultureItems(content));
        }

        // If we didn't find a published version, we fall back to looking for an unpublished
        // version of the content, which is slower
        if (_contentService.GetById(key) is { } entity) {
            return Ok(await _backOfficeHelper.GetCultureItems(entity));
        }

        // If neither is found, we return a 404 response
        return NotFound();

    }

    [HttpGet("serverVariables")]
    [EndpointSummary("Returns the server variables.")]
    [EndpointDescription("Returns the server variables including the version, cache buster, and settings.")]
    public RedirectsServerVariablesModel GetServerVariables() {
        return new RedirectsServerVariablesModel {
            Version = RedirectsPackage.InformationalVersion,
            CacheBuster = RedirectsPackage.InformationalVersion.ToMd5Hash(),
            Settings = _settings.Value
        };
    }

    [HttpGet("users/current")]
    [EndpointSummary("Returns the current user.")]
    [EndpointDescription("Returns the current user including their ID, key, and groups.")]
    public ActionResult<RedirectsUserModel> GetCurrentUser() {
        IUser user = _backOfficeHelper.CurrentUser ?? throw new InvalidOperationException("No current user found.");
        return Ok(new RedirectsUserModel {
            Id = user.Id,
            Key = user.Key,
            Groups = user.Groups.SelectList(x => x.Alias)
        });
    }

    #endregion

    #region Private helper methods

    private void TryUpdateDestination(IRedirect redirect) {

        if (!_umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbraco)) return;

        switch (redirect.Destination.Type) {

            case RedirectDestinationType.Content:
                if (redirect.Destination.Key != Guid.Empty) {
                    if (umbraco.Content.GetById(redirect.Destination.Key) is { } content) {
                        redirect.Destination.Id = content.Id;
                        redirect.Destination.Url = content.Url(culture: redirect.Destination.Culture);
                        redirect.Destination.Name = content.Name(culture: redirect.Destination.Culture);
                    }
                }
                break;

            case RedirectDestinationType.Media:
                if (redirect.Destination.Key != Guid.Empty && redirect.Destination.Id == 0) {
                    if (umbraco.Media.GetById(redirect.Destination.Key) is { } media) {
                        redirect.Destination.Id = media.Id;
                    }
                }
                break;

            case RedirectDestinationType.Url:
                if (!redirect.Destination.Url.StartsWith('/')) return;
                if (redirect.Destination.Url.StartsWith("/media/")) {
                    IMedia? media = StaticServiceProvider.Instance.GetRequiredService<IMediaService>().GetMediaByPath(redirect.Destination.Url);
                    if (media is not null && umbraco.Media.GetById(media.Key) is { } published) {
                        redirect.Destination = new RedirectDestination(published);
                    }
                } else {

                    // TODO: might need to specify the start node here????

                    Guid? key = _documentUrlService.GetDocumentKeyByRoute(redirect.Destination.Url, null, null, false);
                    if (key is not null && umbraco.Content.GetById(key.Value) is { } content) {
                        redirect.Destination = new RedirectDestination(content);
                    }
                }
                break;

        }

    }

    private new JsonResult Ok(object value) {
        return new JsonResult(value);
    }

    private JsonResult Error(RedirectsException ex) {

        string message;
        object? data = null;

        switch (ex) {

            case RedirectsUserException uex:
                message = _backOfficeHelper.Localize(uex);
                data = uex.Data;
                break;

            case RedirectsLocalizedException lex:
                message = lex.GetLocalizedMessage(_localizedTextService, _backOfficeHelper.CurrentCulture);
                break;

            default:
                // TODO: should we really return the exception message here?
                message = ex.Message;
                break;



        }

        // Initialize a new error model based on the exception
        RedirectsErrorModel body = new(message, data);

        return new JsonResult(body) {
            StatusCode = (int) ex.StatusCode
        };

    }

    #endregion

}