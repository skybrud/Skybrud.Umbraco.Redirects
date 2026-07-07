using System;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft.Extensions;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Skybrud.Umbraco.Redirects.wwwroot.Modals.Outbound;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Factories;

/// <summary>
/// Factory class for various models used within the redirects package.
/// </summary>
public class RedirectsModelFactory {

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
    public RedirectsModelFactory(IUmbracoContextAccessor umbracoContextAccessor) {
        _umbracoContextAccessor = umbracoContextAccessor;
    }

    #endregion

    #region Member methods

    /// <summary>
    /// Parses the specified <see cref="JObject"/> into an instance of <see cref="IOutboundRedirect"/>.
    /// </summary>
    /// <param name="json">The JSON object representing the outbound redirect.</param>
    /// <returns>An instance of <see cref="IOutboundRedirect"/>.</returns>
    public virtual IOutboundRedirect? ParseOutboundRedirect(JObject? json) {

        if (json == null) return null;

        // Gets the type of the redirect
        RedirectType type = json.GetBoolean("permanent") ? RedirectType.Permanent : RedirectType.Temporary;

        // Get whether query string forwarding should be enabled
        bool forward = json.GetBoolean("forward");

        // Parse the destination
        IRedirectDestination? destination = json.GetObject("destination", ParseDestination);
        if (destination is null) return null;

        // Look up the current URL for content and media
        switch (destination.Type) {

            case RedirectDestinationType.Content:
                IPublishedContent? content = UmbracoContext.Content.GetById(destination.Key);
                if (content is not null) destination.Url = content.Url();
                break;

            case RedirectDestinationType.Media:
                IPublishedContent? media = UmbracoContext.Media.GetById(destination.Key);
                if (media is not null) destination.Url = media.Url();
                break;

        }

        // If we don't have a valid URL at this point, we should just return null
        if (string.IsNullOrWhiteSpace(destination.Url)) return null;

        // Initialize and return a new outbound redirect
        return new OutboundRedirect(type, forward, destination);

    }

    /// <summary>
    /// Parses the specified <see cref="JObject"/> into an instance of <see cref="IRedirectDestination"/>.
    /// </summary>
    /// <param name="json">The JSON object representing the destination.</param>
    /// <returns>An instance of <see cref="IRedirectDestination"/>.</returns>
    public virtual IRedirectDestination ParseDestination(JObject json) {

        int id = json.GetInt32("id");
        Guid key = json.GetRequiredGuid("key");
        string? name = json.GetString("name").NullIfWhiteSpace();
        string? url = json.GetString("url").NullIfWhiteSpace();
        string? query = json.GetString("query").NullIfWhiteSpace();
        string? fragment = json.GetString("fragment").NullIfWhiteSpace();
        RedirectDestinationType type = json.GetEnum<RedirectDestinationType>("type");
        string? culture = json.GetString("culture").NullIfWhiteSpace();

        return new RedirectDestination {
            Id = id,
            Key = key,
            Name = name,
            Url = url ?? "", // TODO: Should we throw an exception if the URL is null or whitespace?
            Query = query,
            Fragment = fragment,
            Type = type,
            Culture = culture
        };

    }

    #endregion

}