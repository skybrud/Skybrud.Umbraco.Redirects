using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Skybrud.Essentials.Reflection;
using Skybrud.Umbraco.Redirects.Dashboards;
using Umbraco.Cms.Core.Dashboards;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Helpers {
    
    public class RedirectsBackOfficeHelper {
        
        private readonly IRuntimeState _runtimeState;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LinkGenerator _linkGenerator;

        public RedirectsBackOfficeHelper(IRuntimeState runtimeState, IHttpContextAccessor httpContextAccessor, LinkGenerator linkGenerator) {
            _runtimeState = runtimeState;
            _httpContextAccessor = httpContextAccessor;
            _linkGenerator = linkGenerator;
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

    }

}