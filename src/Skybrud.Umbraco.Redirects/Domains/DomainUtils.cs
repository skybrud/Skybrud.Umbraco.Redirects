using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Routing;

namespace Skybrud.Umbraco.Redirects.Domains {
    
    /// <summary>
    /// Static utility class with helper methods related to domains.
    /// </summary>
    public static class DomainUtils {
        
        /// <summary>
        /// Finds the domain matching the specified uri using the domain service.
        /// </summary>
        /// <param name="current">The uri, or null.</param>
        /// <returns>The domain</returns>
        public static IDomain FindDomainForUri(Uri current) {
            var domains = ApplicationContext.Current.Services.DomainService.GetAll(false);
            if (domains == null || !domains.Any()) return null;
            DomainAndUri domain = DomainForUri(domains, current);
            return domain?.UmbracoDomain;
        }

        /// <summary>
        /// Finds the domain that best matches a specified uri, into a group of domains.
        /// </summary>
        /// <param name="domains">The group of domains.</param>
        /// <param name="current">The uri, or null.</param>
        /// <returns>The domain and its normalized uri, that best matches the specified uri.</returns>
        /// <remarks>Copied from Umbraco core, since it's an internal method there.</remarks>
        /// <see>
        ///     <cref>https://github.com/umbraco/Umbraco-CMS/blob/22bd6cd989e2596b69339d9b344e14bcc759e82b/src/Umbraco.Web/Routing/DomainHelper.cs#L117</cref>
        /// </see>
        private static DomainAndUri DomainForUri(IEnumerable<IDomain> domains, Uri current) {
            
            // sanitize the list to have proper uris for comparison (scheme, path end with /)
            // we need to end with / because example.com/foo cannot match example.com/foobar
            // we need to order so example.com/foo matches before example.com/
            string scheme = current == null ? Uri.UriSchemeHttp : current.Scheme;
            DomainAndUri[] domainsAndUris = domains
                .Select(x => new DomainAndUri(SanitizeForBackwardCompatibility(x), scheme))
                .OrderByDescending(d => d.Uri.ToString())
                .ToArray();

            // Just return null if there are no domains
            if (domainsAndUris.Length == 0) return null;

            // take the first one by default (what else can we do?)
            if (current == null) return domainsAndUris[0]; 
            
            // look for the first domain that would be the base of the current url
            // ie current is www.example.com/foo/bar, look for domain www.example.com
            Uri currentWithSlash = current.EndPathWithSlash();
            DomainAndUri domainAndUri = domainsAndUris.FirstOrDefault(d => d.Uri.EndPathWithSlash().IsBaseOf(currentWithSlash));
            if (domainAndUri != null) return domainAndUri;

            // if none matches, try again without the port
            // ie current is www.example.com:1234/foo/bar, look for domain www.example.com
            domainAndUri = domainsAndUris.FirstOrDefault(d => d.Uri.EndPathWithSlash().IsBaseOf(currentWithSlash.WithoutPort()));
            return domainAndUri;

        }

        /// <summary>
        /// Sanitize a Domain.
        /// </summary>
        /// <param name="domain">The Domain to sanitize.</param>
        /// <returns>The sanitized domain.</returns>
        /// <remarks>Copied from Umbraco core, since it's an internal method there.</remarks>
        /// <see>
        ///     <cref>https://github.com/umbraco/Umbraco-CMS/blob/22bd6cd989e2596b69339d9b344e14bcc759e82b/src/Umbraco.Web/Routing/DomainHelper.cs#L197</cref>
        /// </see>
        private static IDomain SanitizeForBackwardCompatibility(IDomain domain) {

            HttpContext context = HttpContext.Current;

            if (context == null || !domain.DomainName.StartsWith("/")) return domain;
            
            // turn "/en" into "http://whatever.com/en" so it becomes a parseable uri
            string authority = context.Request.Url.GetLeftPart(UriPartial.Authority);
            domain.DomainName = authority + domain.DomainName;

            return domain;

        }

    }

}