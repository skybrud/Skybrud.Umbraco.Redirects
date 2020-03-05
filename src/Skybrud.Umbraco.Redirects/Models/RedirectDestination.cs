using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.Composing;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Class representing the destination of a redirect.
    /// </summary>
    public class RedirectDestination {

        private string _url;

        #region Properties

        /// <summary>
        /// Gets the ID of the selected content or media. If an URL has been selected, this will return <c>0</c>.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets the GUID key of the selected content or media. If an URL has been selected, this will return <c>null</c>.
        /// </summary>
        [JsonProperty("key")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets the URL of the destination. Since the URL of a content or media item may change over time (eg. if
        /// renamed or moved), this property will attempt to retrieve the current URL from the relevant Umbraco cache.
        /// 
        /// If <see cref="Type"/> is <see cref="RedirectDestinationType.Content"/>, the URL of the content item will be
        /// retrieved through the content cache (if available). In a similar way, if <see cref="Type"/> is
        /// <see cref="RedirectDestinationType.Media"/> the URL of the media item will be retrieved through the media cache
        /// (if available). The original URL as saved in Umbraco can be accessed through the <see cref="RawUrl"/>
        /// property.
        /// </summary>
        [JsonProperty("url")]
        public string Url {
            get => _url ?? (_url = GetCalculatedUrl());
            set => _url = value;
        }

        [JsonProperty("name")]
        public string Name => GetCalculatedName();

        /// <summary>
        /// Gets the type of the destination.
        /// </summary>
        [JsonProperty("type")]
        public RedirectDestinationType Type { get; set; }

        /// <summary>
        /// Gets whether the link is valid.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => string.IsNullOrWhiteSpace(Url) == false;

        /// <summary>
        /// Gets the raw URL as saved in Umbraco. The URL may be wrong if referencing content or media that has been
        /// renamed, moved or similar.
        /// </summary>
        [JsonIgnore]
        public string RawUrl { get; }

        #endregion

        #region Constructors

        internal RedirectDestination(RedirectDestination destination) {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            Id = destination.Id;
            Key = destination.Key;
            RawUrl = destination.Url;
            Type = destination.Type;
        }

        internal RedirectDestination() { }

        /// <summary>
        /// Initializes a new link picker item.
        /// </summary>
        /// <param name="id">The ID of the content or media item.</param>
        /// <param name="key">The GUID ID of the destination.</param>
        /// <param name="url">The URL of the link.</param>
        /// <param name="type">The type of the link - either <see cref="RedirectDestinationType.Content"/>,
        /// <see cref="RedirectDestinationType.Media"/> or <see cref="RedirectDestinationType.Url"/>.</param>
        public RedirectDestination(int id, Guid key, string url, RedirectDestinationType type) {
            Id = id;
            Key = key;
            RawUrl = url;
            Type = type;
        }

        /// <summary>
        /// Initializes a new link picker item.
        /// </summary>
        /// <param name="id">The ID of the content or media item.</param>
        /// <param name="key">The GUID ID of the destination.</param>
        /// <param name="url">The URL of the link.</param>
        /// <param name="rawUrl">The raw URL of the link.</param>
        /// <param name="mode">The mode of the link - either <see cref="RedirectDestinationType.Content"/>,
        /// <see cref="RedirectDestinationType.Media"/> or <see cref="RedirectDestinationType.Url"/>.</param>
        public RedirectDestination(int id, Guid key, string url, string rawUrl, RedirectDestinationType mode) {
            Id = id;
            _url = url;
            Key = key;
            RawUrl = rawUrl;
            Type = mode;
        }

        private RedirectDestination(JObject obj) {
            Id = obj.GetInt32("id");
            Key = obj.GetGuid("key");
            RawUrl = obj.GetString("url");
            Type = obj.GetEnum("mode", RedirectDestinationType.Url);
        }

        #endregion

        #region Member methods

        protected virtual string GetCalculatedName() {

            // If we dont have a valid UmbracoContext (eg. during Examine indexing), we return null instead (no name)
            if (Current.UmbracoContext == null) return null;

            // Look up the actual URL for content and media
            switch (Type) {
                case RedirectDestinationType.Content: {
                    IPublishedContent content = Current.UmbracoContext.Content.GetById(Id);
                    return content?.Name;
                }
                case RedirectDestinationType.Media: {
                    IPublishedContent media = Current.UmbracoContext.Media.GetById(Id);
                    return media?.Name;
                }
            }

            // Use the raw URL as a fallback
            return null;

        }

        /// <summary>
        /// Method for calculating the current destination URL of the redirect if the destination is either a content item or media item.
        /// </summary>
        /// <returns>The calculated URL.</returns>
        protected virtual string GetCalculatedUrl() {

            // TODO: the calculated URL should support varients/cultures

            // If we dont have a valid UmbracoContext (eg. during Examine indexing), we simply return the raw URL
            if (Current.UmbracoContext == null) return RawUrl;
            
            // Look up the actual URL for content and media
            switch (Type) {
                case RedirectDestinationType.Content: {
                    IPublishedContent content = Current.UmbracoContext.Content.GetById(Id);
                    return content == null ? RawUrl : content.Url;
                }
                case RedirectDestinationType.Media: {
                    IPublishedContent media = Current.UmbracoContext.Media.GetById(Id);
                    return media == null ? RawUrl : media.Url;
                }
            }
            
            // Use the raw URL as a fallback
            return RawUrl;
        
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses the specified <paramref name="obj"/> into an instance of <see cref="RedirectDestination"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        public static RedirectDestination Parse(JObject obj) {
            return obj == null ? null : new RedirectDestination(obj);
        }

        /// <summary>
        /// Initializes a new link item from the specified <paramref name="content"/> representing a content
        /// item.
        /// </summary>
        /// <param name="content">An instance of <see cref="IPublishedContent"/> representing a content item.</param>
        /// <returns>The created <see cref="RedirectDestination"/> instance.</returns>
        public static RedirectDestination GetFromContent(IPublishedContent content) {
            if (content == null) throw new ArgumentNullException(nameof(content));
            return new RedirectDestination(content.Id, content.Key, content.Url, content.Url, RedirectDestinationType.Content);
        }

        /// <summary>
        /// Initializes a new link item from the specified <paramref name="media"/> representing a media item.
        /// </summary>
        /// <param name="media">An instance of <see cref="IPublishedContent"/> representing a media item.</param>
        /// <returns>The created <see cref="RedirectDestination"/> instance.</returns>
        public static RedirectDestination GetFromMedia(IPublishedContent media) {
            if (media == null) throw new ArgumentNullException(nameof(media));
            return new RedirectDestination(media.Id, media.Key, media.Url, media.Url, RedirectDestinationType.Media);
        }

        /// <summary>
        /// Initializes a new link picker item from the specified <paramref name="url"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="url">The URL of the link.</param>
        /// <param name="name">The name (text) of the link.</param>
        /// <returns>The created <see cref="RedirectDestination"/> instance.</returns>
        public static RedirectDestination GetFromUrl(string url, string name = null) {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            return new RedirectDestination(0, Guid.Empty, url, RedirectDestinationType.Url);
        }

        #endregion

    }

}