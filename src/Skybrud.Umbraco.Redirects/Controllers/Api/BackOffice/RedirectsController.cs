using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Security.Extensions;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Config;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Helpers;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Api;
using Skybrud.Umbraco.Redirects.Services;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Routing;
using Umbraco.Extensions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Controllers.Api.BackOffice;

[ApiController]
[BackOfficeRoute("skybrud/redirects")]
[Authorize(Policy = AuthorizationPolicies.SectionAccessContent)]
[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Skybrud Redirects")]
public class RedirectsController : Controller {

    private readonly ILogger<RedirectsController> _logger;
    private readonly IOptions<RedirectsSettings> _settings;
    private readonly ILocalizedTextService _localizedTextService;
    private readonly IRedirectsService _redirectsService;
    private readonly RedirectsBackOfficeHelper _backOfficeHelper;
    private readonly IUmbracoContextAccessor _umbracoContextAccessor;
    private readonly IDocumentUrlService _documentUrlService;
    private readonly ILanguageService _languageService;

    #region Constructors

    public RedirectsController(ILogger<RedirectsController> logger, IOptions<RedirectsSettings> settings, ILocalizedTextService localizedTextService, IRedirectsService redirectsService, RedirectsBackOfficeHelper backOfficeHelper, IUmbracoContextAccessor umbracoContextAccessor, IDocumentUrlService documentUrlService, ILanguageService languageService) {
        _logger = logger;
        _settings = settings;
        _localizedTextService = localizedTextService;
        _redirectsService = redirectsService;
        _backOfficeHelper = backOfficeHelper;
        _umbracoContextAccessor = umbracoContextAccessor;
        _documentUrlService = documentUrlService;
        _languageService = languageService;
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
    public object Index(int page = 1, int limit = 20, Guid? rootNodeKey = null, string? type = null, string? text = null) {

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

    [HttpPut("")]
    public object AddRedirect([FromBody] AddRedirectOptions options) {

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
    public object GetRedirect(int id) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectById(id) ?? throw new RedirectNotFoundException(id);

            // Map the result for the API
            return Ok(_backOfficeHelper.Map(redirect));

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed getting redirect with ID '{Id}'.", id);

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpGet("{key:guid}")]
    public object GetRedirect(Guid key) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectByKey(key) ?? throw new RedirectNotFoundException(key);

            // Map the result for the API
            return Ok(_backOfficeHelper.Map(redirect));

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed getting redirect with GUID key '{Key}'.", key);

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpPatch("{key:guid}")]
    public object EditRedirect(Guid key, [FromBody] EditRedirectOptions options) {

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
            return Ok(_backOfficeHelper.Map(redirect));

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed updating redirect with GUID key '{Key}'.", key);

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpDelete("{id:int}")]
    public object DeleteRedirect(int id) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectById(id) ?? throw new RedirectNotFoundException(id);

            // Delete the redirect
            _redirectsService.DeleteRedirect(redirect);

            // Map the result for the API
            return Ok(_backOfficeHelper.Map(redirect));

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed deleting redirect with ID '{Id}'.", id);

            // Generate the error response
            return Error(ex);

        }

    }

    [HttpDelete("{key:guid}")]
    public object DeleteRedirect(Guid key) {

        try {

            // Get a reference to the redirect
            IRedirect redirect = _redirectsService.GetRedirectByKey(key) ?? throw new RedirectNotFoundException(key);

            // Delete the redirect
            _redirectsService.DeleteRedirect(redirect);

            // Map the result for the API
            return Ok(_backOfficeHelper.Map(redirect));

        } catch (RedirectsException ex) {

            if (!ex.Is404) _logger.LogError(ex, "Failed deleting redirect with GUID key '{Key}'.", key);

            // Generate the error response
            return Error(ex);

        }

    }

    /// <summary>
    /// Gets a list of root nodes based on the domains added to Umbraco. A root node will only be included in the
    /// list once - even if it has been assigned multiple domains.
    /// </summary>
    [HttpGet]
    [Route("rootNodes")]
    public ActionResult GetRootNodes() {

        IReadOnlyList<RedirectRootNode> rootNodes = _redirectsService.GetRootNodes();

        return new JsonResult(new {
            total = rootNodes.Count,
            items = rootNodes.Select(x => new ApiRootNode(x))
        });

    }

    /// <summary>
    /// Returns a list of cultures for the content node with the specified <paramref name="key"/>. If the node does not vary by culture, an empty list will be returned instead.
    /// </summary>
    /// <param name="key">The GUID key of the content node.</param>
    /// <returns>A list of cultures.</returns>
    [HttpGet("content/{key:guid}/cultures")]
    public object GetCultures(Guid key) {

        // Get the content node in the cache
        IPublishedContent? content = _umbracoContextAccessor.GetRequiredUmbracoContext().Content.GetById(key);
        if (content is null) return NotFound();

        // Get the cultures for the content node
        IReadOnlyDictionary<string, PublishedCultureInfo> cultures = content.Cultures;

        // If the content node does not vary by culture, return an empty list
        if (cultures.Count == 1 && cultures.ContainsKey("")) return Array.Empty<object>();

        // Return the cultures with some additional info (alias, name, node name and URL)
        return cultures.Select(x => new {
            alias = x.Key,
            name = _languageService.GetAsync(x.Key).Result?.CultureName,
            nodeName = content.Name(culture: x.Key),
            url = content.Url(culture: x.Key)
        });

    }

    [HttpGet]
    [Route("serverVariables")]
    public object GetServerVariables() {
        return new {
            version = RedirectsPackage.InformationalVersion,
            cacheBuster = RedirectsPackage.InformationalVersion.ToMd5Hash(),
            dashboard = new {
                limit = _settings.Value.Dashboard.Limit
            }
        };
    }

    [HttpGet]
    [Route("package")]
    public object GetPackage() {

        StringBuilder sb = new();

        sb.AppendLine("export class RedirectsPackage {");
        sb.AppendLine("    static get version() {");
        sb.AppendLine("        return \"" + RedirectsPackage.InformationalVersion + "\";");
        sb.AppendLine("    }");
        sb.AppendLine("    static get cacheBuster() {");
        sb.AppendLine("        return \"" + RedirectsPackage.InformationalVersion.ToMd5Hash() + "\";");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        sb.AppendLine("export default RedirectsPackage;");

        return new ContentResult {
            StatusCode = 200,
            ContentType = "text/javascript",
            Content = sb.ToString()
        };

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

        // Initialize a new error model based on the exception
        ApiError body = new(ex);

        if (ex is RedirectsLocalizedException lex) {
            body.Error = lex.GetLocalizedMessage(_localizedTextService, _backOfficeHelper.CurrentCulture);
        }

        return new JsonResult(body) {
            StatusCode = (int) ex.StatusCode
        };

    }

    #endregion

}