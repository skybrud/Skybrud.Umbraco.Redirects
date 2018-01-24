using System;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.Umbraco.Redirects.Extensions {

    public static class PublishedContentExtensions {

        public static OutboundRedirect GetOutboundRedirect(this IPublishedContent content) {
            return GetOutboundRedirect(content, "skyRedirect");
        }

        public static OutboundRedirect GetOutboundRedirect(this IPublishedContent content, string propertyAlias) {
            if (String.IsNullOrWhiteSpace(propertyAlias)) throw new ArgumentNullException(nameof(propertyAlias));
            OutboundRedirect redirect = content.GetPropertyValue(propertyAlias) as OutboundRedirect;
            return redirect ?? new OutboundRedirect();
        }
        
    }

}