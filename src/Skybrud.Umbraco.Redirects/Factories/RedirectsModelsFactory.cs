using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

// ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault

namespace Skybrud.Umbraco.Redirects.Factories {

    /// <summary>
    /// Factory class for various models used within the redirects package.
    /// </summary>
    public class RedirectsModelsFactory {

        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        #region Properties

        /// <summary>
        /// Gets a reference to the current <see cref="IUmbracoContext"/>, if any.
        /// </summary>
        protected IUmbracoContext UmbracoContext => _umbracoContextAccessor.GetRequiredUmbracoContext();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance based on the specified <see cref="IUmbracoContextAccessor"/>.
        /// </summary>
        /// <param name="umbracoContextAccessor">An instance of <see cref="IUmbracoContextAccessor"/>.</param>
        public RedirectsModelsFactory(IUmbracoContextAccessor umbracoContextAccessor) {
            _umbracoContextAccessor = umbracoContextAccessor;
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Creates and returns a new <see cref="OutboundRedirect"/> instance.
        /// </summary>
        /// <param name="json">A <see cref="JObject"/> instance representing the outbound redirect.</param>
        /// <returns>An instance of <see cref="OutboundRedirect"/>.</returns>
        public virtual IOutboundRedirect? CreateOutboundRedirect(JObject? json) {

            if (json == null) return null;

            // Gets the type of the redirect
            RedirectType type = json.GetBoolean("permanent") ? RedirectType.Permanent : RedirectType.Temporary;

            // Get whether query string forwarding should be enabled
            bool forward = json.GetBoolean("forward");

            // Parse the destination
            RedirectDestination? destination = json.GetObject("destination", RedirectDestination.Parse);
            if (destination is not { IsValid: true }) return null;

            // Look up the current URL for content and media
            switch (destination.Type) {

                case RedirectDestinationType.Content:
                    IPublishedContent? content = UmbracoContext.Content?.GetById(destination.Key);
                    if (content != null) destination.Url = content.Url();
                    break;

                case RedirectDestinationType.Media:
                    IPublishedContent? media = UmbracoContext.Media?.GetById(destination.Key);
                    if (media != null) destination.Url = media.Url();
                    break;

            }

            // Initialize and return a new outbound redirect
            return new OutboundRedirect(type, forward, destination, json);

        }

        #endregion

    }

}