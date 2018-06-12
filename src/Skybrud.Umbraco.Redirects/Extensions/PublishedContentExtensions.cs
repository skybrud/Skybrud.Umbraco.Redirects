using System;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.Umbraco.Redirects.Extensions {

    /// <summary>
    /// Static class with extenion methods for working with <see cref="IPublishedContent"/> and redirects.
    /// </summary>
    public static class PublishedContentExtensions {

        /// <summary>
        /// Gets an instance of <see cref="OutboundRedirect"/> representing the outbound redirect from the property
        /// with alias <c>skyRedirect</c>.
        /// 
        /// This method will ensure that you'll always get an instance of <see cref="OutboundRedirect"/> even though
        /// the property may not hold a redirect. You can then use the <see cref="OutboundRedirect.IsValid"/> property
        /// to check that the redirect is in fact valid.
        /// </summary>
        /// <param name="content">The content item holding the property.</param>
        /// <returns>An instance of <see cref="OutboundRedirect"/>.</returns>
        public static OutboundRedirect GetOutboundRedirect(this IPublishedContent content) {
            return GetOutboundRedirect(content, "skyRedirect");
        }

        /// <summary>
        /// Gets an instance of <see cref="OutboundRedirect"/> representing the outbound redirect from the property
        /// with specified alias <paramref name="propertyAlias"/>.
        /// 
        /// This method will ensure that you'll always get an instance of <see cref="OutboundRedirect"/> even though
        /// the property may not hold a redirect. You can then use the <see cref="OutboundRedirect.IsValid"/> property
        /// to check that the redirect is in fact valid.
        /// </summary>
        /// <param name="content">The content item holding the property.</param>
        /// <param name="propertyAlias">The alias of the property.</param>
        /// <returns>An instance of <see cref="OutboundRedirect"/>.</returns>
        public static OutboundRedirect GetOutboundRedirect(this IPublishedContent content, string propertyAlias) {
            if (String.IsNullOrWhiteSpace(propertyAlias)) throw new ArgumentNullException(nameof(propertyAlias));
            OutboundRedirect redirect = content.GetPropertyValue(propertyAlias) as OutboundRedirect;
            return redirect ?? new OutboundRedirect();
        }
        
    }

}