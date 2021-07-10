using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ClientDependency.Core.Config;
using Skybrud.Essentials.Reflection;
using Skybrud.Umbraco.Redirects.Controllers.Api;
using Skybrud.Umbraco.Redirects.Dashboards;
using Umbraco.Core;
using Umbraco.Core.Dashboards;
using Umbraco.Web;

namespace Skybrud.Umbraco.Redirects.Helpers {
    
    public class RedirectsBackOfficeHelper {
        
        private readonly IRuntimeState _runtimeState;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public RedirectsBackOfficeHelper(IRuntimeState runtimeState, IUmbracoContextAccessor umbracoContextAccessor) {
            _runtimeState = runtimeState;
            _umbracoContextAccessor = umbracoContextAccessor;
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
            return $"{version1}.{_runtimeState.Level}.{ClientDependencySettings.Instance.Version}.{version2}".GenerateHash();
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
            string baseUrl = null;
            if (TryCreateUrlHelper(out UrlHelper url)) {
                baseUrl = url.GetUmbracoApiService<RedirectsController>("Dummy").TrimEnd("Dummy");
            }
            
            // Append the "redirects" dictionary to "skybrud"
            return new Dictionary<string, object> {
                {"baseUrl", baseUrl },
                {"cacheBuster", GetCacheBuster()},
                {"version", GetInformationVersion()}
            };

        }

        private bool TryCreateUrlHelper(out UrlHelper helper) {

            // Get the current HTTP context via the Umbraco context accessor
            HttpContextBase http = _umbracoContextAccessor.UmbracoContext.HttpContext;
            if (http == null) {
                helper = null;
                return false;
            }

            // Initialize a new URL helper
            helper = new UrlHelper(http.Request.RequestContext);
            return true;

        }

    }

}