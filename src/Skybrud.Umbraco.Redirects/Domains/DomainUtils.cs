using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web.Routing;

namespace Skybrud.Umbraco.Redirects.Domains
{
    /// <summary>
    /// Static utility class with helper methods related to domains.
    /// </summary>
    public static class DomainUtils
    {
        /// <summary>
        /// Finds the domain matching the specified uri using the domain service.
        /// </summary>
        /// <param name="current">The uri, or null.</param>
        /// <returns>The domain</returns>
        public static IDomain FindDomainForUri(Uri current)
        {
            var domains = ApplicationContext.Current.Services.DomainService.GetAll(true);

            if (domains == null || !domains.Any()) return null;

            var domain = DomainForUri(domains, current);

            if (domain != null) return domain.UmbracoDomain;

            return null;
        }

        /// <summary>
        /// Finds the domain that best matches a specified uri, into a group of domains.
        /// </summary>
        /// <param name="domains">The group of domains.</param>
        /// <param name="current">The uri, or null.</param>
        /// <param name="filter">A function to filter the list of domains, if more than one applies, or <c>null</c>.</param>
        /// <returns>The domain and its normalized uri, that best matches the specified uri.</returns>
        /// <remarks>Copied from Umbraco core, since it's an internal method there.</remarks>
        private static DomainAndUri DomainForUri(IEnumerable<IDomain> domains, Uri current, Func<DomainAndUri[], DomainAndUri> filter = null)
        {
            // sanitize the list to have proper uris for comparison (scheme, path end with /)
            // we need to end with / because example.com/foo cannot match example.com/foobar
            // we need to order so example.com/foo matches before example.com/
            var scheme = current == null ? Uri.UriSchemeHttp : current.Scheme;
            var domainsAndUris = domains
                .Where(d => d.IsWildcard == false)
                .Select(SanitizeForBackwardCompatibility)
                .Select(d => new DomainAndUri(d, scheme))
                .OrderByDescending(d => d.Uri.ToString())
                .ToArray();

            if (domainsAndUris.Any() == false)
                return null;

            DomainAndUri domainAndUri;
            if (current == null)
            {
                // take the first one by default (what else can we do?)
                domainAndUri = domainsAndUris.First(); // .First() protected by .Any() above
            }
            else
            {
                // look for the first domain that would be the base of the current url
                // ie current is www.example.com/foo/bar, look for domain www.example.com
                var currentWithSlash = current.EndPathWithSlash();
                domainAndUri = domainsAndUris
                    .FirstOrDefault(d => d.Uri.EndPathWithSlash().IsBaseOf(currentWithSlash));
                if (domainAndUri != null) return domainAndUri;

                // if none matches, try again without the port
                // ie current is www.example.com:1234/foo/bar, look for domain www.example.com
                domainAndUri = domainsAndUris
                    .FirstOrDefault(d => d.Uri.EndPathWithSlash().IsBaseOf(currentWithSlash.WithoutPort()));
                if (domainAndUri != null) return domainAndUri;

                // if none matches, then try to run the filter to pick a domain
                if (filter != null)
                {
                    domainAndUri = filter(domainsAndUris);
                    // if still nothing, pick the first one?
                    // no: move that constraint to the filter, but check
                    if (domainAndUri == null)
                        throw new InvalidOperationException("The filter returned null.");
                }
            }

            return domainAndUri;
        }

        /// <summary>
        /// Sanitize a Domain.
        /// </summary>
        /// <param name="domain">The Domain to sanitize.</param>
        /// <returns>The sanitized domain.</returns>
        /// <remarks>Copied from Umbraco core, since it's an internal method there.</remarks>
        private static IDomain SanitizeForBackwardCompatibility(IDomain domain)
        {
            var context = HttpContext.Current;

            if (context != null && domain.DomainName.StartsWith("/"))
            {
                // turn "/en" into "http://whatever.com/en" so it becomes a parseable uri
                var authority = context.Request.Url.GetLeftPart(UriPartial.Authority);
                domain.DomainName = authority + domain.DomainName;
            }

            return domain;
        }
    }
}