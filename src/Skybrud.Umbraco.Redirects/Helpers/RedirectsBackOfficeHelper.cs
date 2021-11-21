using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Skybrud.Essentials.Reflection;
using Skybrud.Umbraco.Redirects.Dashboards;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Api;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Dashboards;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Helpers {
    
    public class RedirectsBackOfficeHelper {
        
        private readonly IRuntimeState _runtimeState;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;
        private readonly IDomainService _domainService;
        private readonly IContentService _contentService;
        private readonly IMediaService _mediaService;
        private readonly ILocalizedTextService _textService;

        public RedirectsBackOfficeHelper(IRuntimeState runtimeState, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator, IDomainService domainService, IContentService contentService, IMediaService mediaService, ILocalizedTextService textService) {
            _runtimeState = runtimeState;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
            _domainService = domainService;
            _contentService = contentService;
            _mediaService = mediaService;
            _textService = textService;
        }
        
        /// <summary>
        /// Returns the localized value for the key with the specified <paramref name="alias"/> within the <c>redirects</c> area.
        /// </summary>
        /// <param name="alias">The alias of the key.</param>
        /// <returns>The localized value.</returns>
        public string Localize(string alias) {
            return Localize(alias, "redirects");
        }
        
        /// <summary>
        /// Returns the localized value for the key with the specified <paramref name="alias"/> and <paramref name="area"/>.
        /// </summary>
        /// <param name="alias">The alias of the key.</param>
        /// <param name="area">The area in which the key is located.</param>
        /// <returns>The localized value.</returns>
        public string Localize(string alias, string area) {
            return _textService.Localize(area, alias);
        }

        /// <summary>
        /// Returns the access rules for <see cref="RedirectsDashboard"/>.
        /// </summary>
        /// <returns>An array of <see cref="IAccessRule"/>.</returns>
        public virtual IAccessRule[] GetDashboardAccessRules() {
            return Array.Empty<IAccessRule>();
        }

        /// <summary>
        /// Returns a cache buster value based both on Umbraco's own cache buster as well as the current version of
        /// this package. This ensures a new cache buster value when either the ClientDependency version is bumped or
        /// the package is updated.
        /// </summary>
        /// <returns>The cache buster value.</returns>
        public virtual string GetCacheBuster() {
            string version1 = _runtimeState.SemanticVersion.ToSemanticString();
            string version2 = GetInformationVersion();
            return $"{version1}.{_runtimeState.Level}.{version2}".GenerateHash();
        }

        /// <summary>
        /// Gets the information version of the package.
        /// </summary>
        /// <returns>The information version.</returns>
        public string GetInformationVersion() {
            return ReflectionUtils.GetInformationalVersion(GetType().Assembly);
        }

        /// <summary>
        /// Returns a dictonary with server variables for this pacakge, available through <c>Umbraco.Sys.ServerVariables.skybrud.redirects</c> in the backoffice.
        /// </summary>
        /// <returns>An instance of <see cref="Dictionary{TKey,TValue}"/>.</returns>
        public virtual Dictionary<string, object> GetServerVariables() {
            
            // Determine the API base URL
            string baseUrl = "/umbraco/backoffice/Skybrud/Redirects/";
            
            // Append the "redirects" dictionary to "skybrud"
            return new Dictionary<string, object> {
                {"baseUrl", baseUrl },
                {"cacheBuster", GetCacheBuster()},
                {"version", GetInformationVersion()}
            };

        }

        //private bool TryCreateUrlHelper(out UrlHelper helper) {

        //    _linkGenerator.GetUmbracoApiService<RedirectsController>("Dummy").TrimEnd("Dummy");

        //    // Get the current HTTP context via the Umbraco context accessor
        //    HttpContextBase http = _httpContextAccessor.HttpContext.u
        //    if (http == null) {
        //        helper = null;
        //        return false;
        //    }

        //    // Initialize a new URL helper
        //    helper = new UrlHelper(http.Request.RequestContext);
        //    return true;

        //}

        public virtual object Map(RedirectsSearchResult result) {
            
            Dictionary<Guid, RedirectRootNodeModel> rootNodeLookup = new Dictionary<Guid, RedirectRootNodeModel>();
            Dictionary<Guid, IContent> contentLookup = new Dictionary<Guid, IContent>();
            Dictionary<Guid, IMedia> mediaLookup = new Dictionary<Guid, IMedia>();

            IEnumerable<RedirectModel> items = result.Items
                .Select(redirect => Map(redirect, rootNodeLookup, contentLookup, mediaLookup));

            return new {
                result.Pagination,
                items
            };

        }

        public virtual RedirectModel Map(IRedirect redirect)  {
            Dictionary<Guid, RedirectRootNodeModel> rootNodeLookup = new();
            Dictionary<Guid, IContent> contentLookup = new();
            Dictionary<Guid, IMedia> mediaLookup = new();
            return Map(redirect, rootNodeLookup, contentLookup, mediaLookup);
        }

        public virtual IEnumerable<RedirectModel> Map(IEnumerable<IRedirect> redirects) {
            Dictionary<Guid, RedirectRootNodeModel> rootNodeLookup = new();
            Dictionary<Guid, IContent> contentLookup = new();
            Dictionary<Guid, IMedia> mediaLookup = new();
            return redirects.Select(redirect => Map(redirect, rootNodeLookup, contentLookup, mediaLookup));
        }

        private RedirectModel Map(IRedirect redirect, Dictionary<Guid, RedirectRootNodeModel> rootNodeLookup,
            Dictionary<Guid, IContent> contentLookup, Dictionary<Guid, IMedia> mediaLookup)
        {
            
            RedirectRootNodeModel rootNode = null;
            if (redirect.RootKey != Guid.Empty) {

                if (!rootNodeLookup.TryGetValue(redirect.RootKey, out rootNode)) {

                    if (!contentLookup.TryGetValue(redirect.RootKey, out IContent content)) {
                        content = _contentService.GetById(redirect.RootKey);
                        if (content != null) contentLookup.Add(content.Key, content);
                    }
                    var domains = content == null ? null :_domainService.GetAssignedDomains(content.Id, false).Select(x => x.DomainName).ToArray();
                    rootNode = new RedirectRootNodeModel(redirect, content, domains);

                    rootNodeLookup.Add(rootNode.Key, rootNode);

                }
            }

            RedirectDestinationModel destination;
            if (redirect.Destination.Type == RedirectDestinationType.Content) {
                if (!contentLookup.TryGetValue(redirect.Destination.Key, out IContent content)) {
                    content = _contentService.GetById(redirect.Destination.Key);
                    if (content != null) contentLookup.Add(content.Key, content);
                }
                destination = new RedirectDestinationModel(redirect, content);
            } else if (redirect.Destination.Type == RedirectDestinationType.Media) {
                if (!mediaLookup.TryGetValue(redirect.Destination.Key, out IMedia media)) {
                    media = _mediaService.GetById(redirect.Destination.Key);
                    if (media != null) mediaLookup.Add(media.Key, media);
                }
                destination = new RedirectDestinationModel(redirect, media);
            } else {
                destination = new RedirectDestinationModel(redirect);
            }

            return new RedirectModel(redirect, rootNode, destination);

        }

        public virtual ContentApp GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups)  {

            switch (source) {

                case IContent content:
                    return content.ContentType.IsElement ? null : GetDefaultContentApp();

                case IMedia media:
                    return media.ContentType.Alias == Constants.Conventions.MediaTypes.Folder ? null : GetDefaultContentApp();

                default:
                    return null;
                
            }

        }

        public virtual ContentApp GetDefaultContentApp() {

            return new() {
                Alias = "redirects",
                Name = "Redirects",
                Icon = "icon-arrow-right",
                View = $"/App_Plugins/Skybrud.Umbraco.Redirects/Views/ContentApp.html?v={GetCacheBuster()}",
                Weight = 99
            };

        }

    }

}