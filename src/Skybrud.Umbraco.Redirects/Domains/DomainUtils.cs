using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Composing;
using Umbraco.Web.Routing;

namespace Skybrud.Umbraco.Redirects.Domains {
    
    /// <summary>
    /// Static utility class with helper methods related to domains.
    /// </summary>
    public static class DomainUtils {
        
        /// <summary>
        /// Finds the domain matching the specified uri using the domain service.
        /// </summary>
        /// <param name="domainService"></param>
        /// <param name="current">The uri, or null.</param>
        /// <returns>The domain</returns>
        public static Domain FindDomainForUri(IDomainService domainService, Uri current) {
            var domains = domainService.GetAll(false);
            if (domains == null || !domains.Any()) return null;
            DomainAndUri domain = SelectDomain(domains.Select(x => new Domain(x.Id, x.DomainName, x.RootContentId.GetValueOrDefault(), new System.Globalization.CultureInfo(x.LanguageIsoCode), x.IsWildcard)), current);
            return domain;
        }

        /// <summary>
        /// Selects the domain that best matches a specified uri and cultures, from a set of domains.
        /// </summary>
        /// <param name="domains">The group of domains.</param>
        /// <param name="uri">An optional uri.</param>
        /// <param name="culture">An optional culture.</param>
        /// <param name="defaultCulture">An optional default culture.</param>
        /// <param name="filter">An optional function to filter the list of domains, if more than one applies.</param>
        /// <returns>The domain and its normalized uri, that best matches the specified uri and cultures.</returns>
        /// <remarks>
        /// TODO: must document and explain this all
        /// <para>If <paramref name="uri"/> is null, pick the first domain that matches <paramref name="culture"/>,
        /// else the first that matches <paramref name="defaultCulture"/>, else the first one (ordered by id), else null.</para>
        /// <para>If <paramref name="uri"/> is not null, look for domains that would be a base uri of the current uri,</para>
        /// <para>If more than one domain matches, then the <paramref name="filter"/> function is used to pick
        /// the right one, unless it is <c>null</c>, in which case the method returns <c>null</c>.</para>
        /// <para>The filter, if any, will be called only with a non-empty argument, and _must_ return something.</para>
        /// <remarks>Copied from Umbraco core, since it's an internal method there.</remarks>
        /// <see>
        ///     <cref>https://github.com/umbraco/Umbraco-CMS/blob/193e24afd256b2994a0900c0e9739f3f3466c036/src/Umbraco.Web/Routing/DomainHelper.cs#L98</cref>
        /// </see>
        /// </remarks>
        internal static DomainAndUri SelectDomain(IEnumerable<Domain> domains, Uri uri, string culture = null, string defaultCulture = null, Func<IReadOnlyCollection<DomainAndUri>, Uri, string, string, DomainAndUri> filter = null)
        {
            // sanitize the list to have proper uris for comparison (scheme, path end with /)
            // we need to end with / because example.com/foo cannot match example.com/foobar
            // we need to order so example.com/foo matches before example.com/
            var domainsAndUris = domains
                .Where(d => d.IsWildcard == false)
                .Select(d => new DomainAndUri(d, uri))
                .OrderByDescending(d => d.Uri.ToString())
                .ToList();

            // nothing = no magic, return null
            if (domainsAndUris.Count == 0)
                return null;

            // sanitize cultures
            culture = culture.NullOrWhiteSpaceAsNull();
            defaultCulture = defaultCulture.NullOrWhiteSpaceAsNull();

            if (uri == null)
            {
                // no uri - will only rely on culture
                return GetByCulture(domainsAndUris, culture, defaultCulture);
            }

            // else we have a uri,
            // try to match that uri, else filter

            // if a culture is specified, then try to get domains for that culture
            // (else cultureDomains will be null)
            // do NOT specify a default culture, else it would pick those domains
            var cultureDomains = SelectByCulture(domainsAndUris, culture, defaultCulture: null);
            IReadOnlyCollection<DomainAndUri> considerForBaseDomains = domainsAndUris;
            if (cultureDomains != null)
            {
                if (cultureDomains.Count == 1) // only 1, return
                    return cultureDomains.First();

                // else restrict to those domains, for base lookup
                considerForBaseDomains = cultureDomains;
            }

            // look for domains that would be the base of the uri
            var baseDomains = SelectByBase(considerForBaseDomains, uri);
            if (baseDomains.Count > 0) // found, return
                return baseDomains.First();

            // if nothing works, then try to run the filter to select a domain
            // either restricting on cultureDomains, or on all domains
            if (filter != null)
            {
                var domainAndUri = filter(cultureDomains ?? domainsAndUris, uri, culture, defaultCulture);
                // if still nothing, pick the first one?
                // no: move that constraint to the filter, but check
                if (domainAndUri == null)
                    throw new InvalidOperationException("The filter returned null.");
                return domainAndUri;
            }

            return null;
        }

        private static bool IsBaseOf(DomainAndUri domain, Uri uri)
            => domain.Uri.EndPathWithSlash().IsBaseOf(uri);

        private static IReadOnlyCollection<DomainAndUri> SelectByBase(IReadOnlyCollection<DomainAndUri> domainsAndUris, Uri uri)
        {
            // look for domains that would be the base of the uri
            // ie current is www.example.com/foo/bar, look for domain www.example.com
            var currentWithSlash = uri.EndPathWithSlash();
            var baseDomains = domainsAndUris.Where(d => IsBaseOf(d, currentWithSlash)).ToList();

            // if none matches, try again without the port
            // ie current is www.example.com:1234/foo/bar, look for domain www.example.com
            var currentWithoutPort = currentWithSlash.WithoutPort();
            if (baseDomains.Count == 0)
                baseDomains = domainsAndUris.Where(d => IsBaseOf(d, currentWithoutPort)).ToList();

            return baseDomains;
        }

        private static IReadOnlyCollection<DomainAndUri> SelectByCulture(IReadOnlyCollection<DomainAndUri> domainsAndUris, string culture, string defaultCulture)
        {
            // we try our best to match cultures, but may end with a bogus domain

            if (culture != null) // try the supplied culture
            {
                var cultureDomains = domainsAndUris.Where(x => x.Culture.Name.InvariantEquals(culture)).ToList();
                if (cultureDomains.Count > 0) return cultureDomains;
            }

            if (defaultCulture != null) // try the defaultCulture culture
            {
                var cultureDomains = domainsAndUris.Where(x => x.Culture.Name.InvariantEquals(defaultCulture)).ToList();
                if (cultureDomains.Count > 0) return cultureDomains;
            }

            return null;
        }

        private static DomainAndUri GetByCulture(IReadOnlyCollection<DomainAndUri> domainsAndUris, string culture, string defaultCulture)
        {
            DomainAndUri domainAndUri;

            // we try our best to match cultures, but may end with a bogus domain

            if (culture != null) // try the supplied culture
            {
                domainAndUri = domainsAndUris.FirstOrDefault(x => x.Culture.Name.InvariantEquals(culture));
                if (domainAndUri != null) return domainAndUri;
            }

            if (defaultCulture != null) // try the defaultCulture culture
            {
                domainAndUri = domainsAndUris.FirstOrDefault(x => x.Culture.Name.InvariantEquals(defaultCulture));
                if (domainAndUri != null) return domainAndUri;
            }

            return domainsAndUris.First(); // what else?
        }

    }

}