using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Skybrud.Essentials.Reflection;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Config;
using Skybrud.Umbraco.Redirects.Dashboards;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Api;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Dashboards;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.ContentEditing;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Helpers {

    /// <summary>
    /// Backoffice helper class for the redirects package.
    /// </summary>
    public class RedirectsBackOfficeHelper {

        #region Properties

        /// <summary>
        /// Gets a reference to the dependencies of this instance.
        /// </summary>
        protected RedirectsBackOfficeHelperDependencies Dependencies { get; }

        /// <summary>
        /// Gets the back-office URL of Umbraco.
        /// </summary>
        public string BackOfficeUrl => Dependencies.GlobalSettings.GetBackOfficePath(Dependencies.HostingEnvironment);

        /// <summary>
        /// Gets a reference to the current backoffice user.
        /// </summary>
        public IUser? CurrentUser => Dependencies.BackOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;

        /// <summary>
        /// Gets a reference to the redirects settings.
        /// </summary>
        public RedirectsSettings Settings => Dependencies.RedirectsSettings.Value;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="dependencies"/>.
        /// </summary>
        /// <param name="dependencies">An instance of <see cref="RedirectsBackOfficeHelperDependencies"/>.</param>
        public RedirectsBackOfficeHelper(RedirectsBackOfficeHelperDependencies dependencies) {
            Dependencies = dependencies;
        }

        #endregion

        #region Member methods

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
            return Dependencies.TextService.Localize(area, alias);
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
            string version1 = Dependencies.RuntimeState.SemanticVersion.ToSemanticString();
            string version2 = GetInformationVersion();
            return $"{version1}.{Dependencies.RuntimeState.Level}.{version2}".GenerateHash();
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

            // Get the backoffice path
            string backOfficePath = Dependencies.GlobalSettings.GetBackOfficePath(Dependencies.HostingEnvironment);

            // Determine the API base URL
            string baseUrl = $"{WebPath.Combine(backOfficePath, "/backoffice/Skybrud/Redirects")}/";

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

        /// <summary>
        /// Maps the specified <paramref name="result"/> to a corresponding object to be returned in the API.
        /// </summary>
        /// <param name="result">The search result to be mapped.</param>
        /// <returns>An instance of <see cref="object"/>.</returns>
        public virtual object Map(RedirectsSearchResult result) {

            Dictionary<Guid, RedirectRootNodeModel> rootNodeLookup = new Dictionary<Guid, RedirectRootNodeModel>();
            Dictionary<Guid, IContent> contentLookup = new Dictionary<Guid, IContent>();
            Dictionary<Guid, IMedia> mediaLookup = new Dictionary<Guid, IMedia>();
            Dictionary<string, ILanguage> languageLookup = new();

            IEnumerable<RedirectModel> items = result.Items
                .Select(redirect => Map(redirect, rootNodeLookup, contentLookup, mediaLookup, languageLookup));

            return new {
                result.Pagination,
                items
            };

        }

        /// <summary>
        /// Maps the specified <paramref name="redirect"/> to a corresponding <see cref="RedirectModel"/> to be returned in the API.
        /// </summary>
        /// <param name="redirect">The redirect to be mapped.</param>
        /// <returns>An instance of <see cref="RedirectModel"/>.</returns>
        public virtual RedirectModel Map(IRedirect redirect)  {
            Dictionary<Guid, RedirectRootNodeModel> rootNodeLookup = new();
            Dictionary<Guid, IContent> contentLookup = new();
            Dictionary<Guid, IMedia> mediaLookup = new();
            Dictionary<string, ILanguage> languageLookup = new();
            return Map(redirect, rootNodeLookup, contentLookup, mediaLookup, languageLookup);
        }

        /// <summary>
        /// Maps the specified collection of <paramref name="redirects"/> to a corresponding colelction of <see cref="RedirectModel"/> to be returned in the API.
        /// </summary>
        /// <param name="redirects">The collection of redirects to be mapped.</param>
        /// <returns>A collection of <see cref="RedirectModel"/>.</returns>
        public virtual IEnumerable<RedirectModel> Map(IEnumerable<IRedirect> redirects) {
            Dictionary<Guid, RedirectRootNodeModel> rootNodeLookup = new();
            Dictionary<Guid, IContent> contentLookup = new();
            Dictionary<Guid, IMedia> mediaLookup = new();
            Dictionary<string, ILanguage> languageLookup = new();
            return redirects.Select(redirect => Map(redirect, rootNodeLookup, contentLookup, mediaLookup, languageLookup));
        }

        private RedirectModel Map(IRedirect redirect, Dictionary<Guid, RedirectRootNodeModel> rootNodeLookup,
            Dictionary<Guid, IContent> contentLookup, Dictionary<Guid, IMedia> mediaLookup, Dictionary<string, ILanguage> languageLookup)
        {

            string backOfficeBaseUrl = Dependencies.GlobalSettings.GetBackOfficePath(Dependencies.HostingEnvironment);

            RedirectRootNodeModel? rootNode = null;
            if (redirect.RootKey != Guid.Empty) {

                if (!rootNodeLookup.TryGetValue(redirect.RootKey, out rootNode)) {

                    if (!contentLookup.TryGetValue(redirect.RootKey, out IContent? content)) {
                        content = Dependencies.ContentService.GetById(redirect.RootKey);
                        if (content != null) contentLookup.Add(content.Key, content);
                    }
                    var domains = content == null ? null : Dependencies.DomainService.GetAssignedDomains(content.Id, false).Select(x => x.DomainName).ToArray();
                    rootNode = new RedirectRootNodeModel(redirect, content, domains, backOfficeBaseUrl);

                    rootNodeLookup.Add(rootNode.Key, rootNode);

                }
            }

            RedirectDestinationModel destination;
            if (redirect.Destination.Type == RedirectDestinationType.Content) {

                if (!contentLookup.TryGetValue(redirect.Destination.Key, out IContent? content)) {
                    content = Dependencies.ContentService.GetById(redirect.Destination.Key);
                    if (content != null) contentLookup.Add(content.Key, content);
                }

                // Initialize a new destination object
                destination = new RedirectDestinationModel(redirect, content);

                // I the destination refers to a specific culture, we fetch some additional information about the culture
                if (redirect.Destination.Culture.HasValue(out string? culture)) {
                    destination.Culture = culture;
                    if (languageLookup.TryGetValue(culture, out ILanguage? language)) {
                        destination.CultureName = language.CultureName;
                    } else {
                        language = Dependencies.LocalizationService.GetLanguageByIsoCode(culture);
                        if (language is not null) {
                            languageLookup.Add(culture, language);
                            destination.CultureName = language.CultureName;
                        }
                    }
                }

                // Look up the current URL of the destination (with respect for the selected culture)
                if (TryGetContent(redirect.Destination.Id, out IPublishedContent? published)) {
                    string url = published.Url(destination.Culture);
                    if (!string.IsNullOrEmpty(url)) destination.Url = url;
                }

                // Set the backoffice URL of the page
                destination.BackOfficeUrl = $"{backOfficeBaseUrl}/#/content/content/edit/{redirect.Destination.Id}";

            } else if (redirect.Destination.Type == RedirectDestinationType.Media) {

                if (!mediaLookup.TryGetValue(redirect.Destination.Key, out IMedia? media)) {
                    media = Dependencies.MediaService.GetById(redirect.Destination.Key);
                    if (media != null) mediaLookup.Add(media.Key, media);
                }

                // Initialize a new destination object
                destination = new RedirectDestinationModel(redirect, media);

                // Look up the current URL of the destination
                if (TryGetMedia(redirect.Destination.Id, out IPublishedContent? published)) {
                    string url = published.Url();
                    if (!string.IsNullOrEmpty(url)) destination.Url = url;
                }

                // Set the backoffice URL of the media
                destination.BackOfficeUrl = $"{backOfficeBaseUrl}/#/content/media/edit/{redirect.Destination.Id}";

            } else {

                destination = new RedirectDestinationModel(redirect);

            }

            return new RedirectModel(redirect, rootNode, destination);

        }

        /// <summary>
        /// Returns the content app for the specified <paramref name="source"/>, or <c>null</c> if no content app should be shown.
        /// </summary>
        /// <param name="source">The source - eg. an instance <see cref="IContent"/>.</param>
        /// <param name="userGroups">The user groups of the current user.</param>
        /// <returns>An instance of <see cref="ContentApp"/>, or <c>null</c> if no content app should be shown.</returns>
        public virtual ContentApp? GetContentAppFor(object source, IEnumerable<IReadOnlyUserGroup> userGroups) {

            // Return null if the content app is disabled via appsettings.json
            if (!Settings.ContentApp.Enabled) return null;

            return source switch {
                IContent content => ShowContentApp(content, userGroups) ? GetDefaultContentApp() : null,
                IMedia media => ShowContentApp(media, userGroups) ? GetDefaultContentApp() : null,
                _ => null
            };

        }

        /// <summary>
        /// Returns whether to show the redirects content app for the specified <paramref name="content"/>.
        /// </summary>
        /// <param name="content">An instance of <see cref="IContent"/>.</param>
        /// <param name="userGroups">The user groups of the current user.</param>
        /// <returns><see langword="true"/> if the content app should be shown; otherwise, <see langword="false"/>.</returns>
        protected virtual bool ShowContentApp(IContent content, IEnumerable<IReadOnlyUserGroup> userGroups) {

            if (content.ContentType.IsElement) return false;
            if (content.TemplateId == null) return false;

            IReadOnlySet<string> show = Settings.ContentApp.Show;

            // ReSharper disable PossibleMultipleEnumeration
            // The underlying type is likely always HashSet<IReadOnlyUserGroup>, so we should care too much about multiple enumerations
            if (userGroups.Any(group => show.Contains("+role/" + group.Alias))) return true;
            if (userGroups.Any(group => show.Contains("-role/" + group.Alias))) return false;
            // ReSharper restore PossibleMultipleEnumeration

            if (show.Contains("-content/" + content.ContentType.Alias)) return false;
            if (show.Contains("+content/" + content.ContentType.Alias)) return true;

            if (show.Contains("-content/*")) return false;
            if (show.Contains("+content/*")) return true;

            return true;

        }

        /// <summary>
        /// Returns whether to show the redirects content app for the specified <paramref name="media"/>.
        /// </summary>
        /// <param name="media">An instance of <see cref="IMedia"/>.</param>
        /// <param name="userGroups">The user groups of the current user.</param>
        /// <returns><see langword="true"/> if the content app should be shown; otherwise, <see langword="false"/>.</returns>
        protected virtual bool ShowContentApp(IMedia media, IEnumerable<IReadOnlyUserGroup> userGroups) {

            if (media.ContentType.Alias == Constants.Conventions.MediaTypes.Folder) return false;

            IReadOnlySet<string> show = Settings.ContentApp.Show;

            // ReSharper disable PossibleMultipleEnumeration
            // The underlying type is likely always HashSet<IReadOnlyUserGroup>, so we should care too much about multiple enumerations
            if (userGroups.Any(group => show.Contains("+role/" + group.Alias))) return true;
            if (userGroups.Any(group => show.Contains("-role/" + group.Alias))) return false;
            // ReSharper restore PossibleMultipleEnumeration

            if (show.Contains("-media/" + media.ContentType.Alias)) return false;
            if (show.Contains("+media/" + media.ContentType.Alias)) return true;

            if (show.Contains("-media/*")) return false;
            if (show.Contains("+media/*")) return true;

            return true;

        }

        /// <summary>
        /// Returns the default content app for the redirects package.
        /// </summary>
        /// <returns>An instance of <see cref="ContentApp"/>.</returns>
        public virtual ContentApp GetDefaultContentApp() {

            return new() {
                Alias = "redirects",
                Name = "Redirects",
                Icon = "icon-arrow-right",
                View = $"/App_Plugins/Skybrud.Umbraco.Redirects/Views/ContentApp.html?v={GetCacheBuster()}",
                Weight = 99
            };

        }

        /// <summary>
        /// Attempts to get the <see cref="IPublishedContent"/> with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The numeric ID.</param>
        /// <param name="result">When this method returns, holds the <see cref="IPublishedContent"/> if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if successful; otherwise, <see langword="false"/>.</returns>
        public virtual bool TryGetContent(int id, [NotNullWhen(true)] out IPublishedContent? result) {

            if (Dependencies.UmbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? context)) {
                result = context.Content?.GetById(id);
                return result is not null;
            }

            result = null;
            return false;

        }

        /// <summary>
        /// Attempts to get the <see cref="IPublishedContent"/> with the specified <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The GUID key.</param>
        /// <param name="result">When this method returns, holds the <see cref="IPublishedContent"/> if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if successful; otherwise, <see langword="false"/>.</returns>
        public virtual bool TryGetContent(Guid key, [NotNullWhen(true)] out IPublishedContent? result) {

            if (Dependencies.UmbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? context)) {
                result = context.Content?.GetById(key);
                return result is not null;
            }

            result = null;
            return false;

        }

        /// <summary>
        /// Attempts to get the media with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">The numeric ID of the media.</param>
        /// <param name="result">When this method returns, holds the <see cref="IPublishedContent"/> representing the media if successful; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if successful; otherwise, <see langword="false"/>.</returns>
        public virtual bool TryGetMedia(int id, [NotNullWhen(true)] out IPublishedContent? result) {

            if (Dependencies.UmbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? context)) {
                result = context.Media?.GetById(id);
                return result is not null;
            }

            result = null;
            return false;

        }

        #endregion

    }

}