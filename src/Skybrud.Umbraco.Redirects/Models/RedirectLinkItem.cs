using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Class representing a single link item.
    /// </summary>
    public class RedirectLinkItem : JsonObjectBase {

        private string _url;

        #region Properties
        
        /// <summary>
        /// Gets the ID of the selected content or media. If an URL has been selected, this will return <c>0</c>.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; }

        /// <summary>
        /// Gets the name of the link.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the URL of the link. Since the URL of a content or media item may change over time (eg. if renamed or
        /// moved), this property will attempt to retrieve the current URL from the relevant Umbraco cache.
        /// 
        /// If <see cref="Mode"/> is <see cref="RedirectLinkMode.Content"/>, the URL of the content item will be
        /// retrieved through the content cache (if available). In a similar way, if <see cref="Mode"/> is
        /// <see cref="RedirectLinkMode.Media"/> the URL of the media item will be retrieved through the media cache
        /// (if available). The original URL as saved in Umbraco can be accessed through the <see cref="RawUrl"/>
        /// property.
        /// </summary>
        [JsonProperty("url")]
        public string Url => _url ?? (_url = GetCalculatedUrl());

        /// <summary>
        /// Gets the mode (or type) of the link.
        /// </summary>
        [JsonProperty("mode")]
        public RedirectLinkMode Mode { get; }

        /// <summary>
        /// Gets whether the link is valid.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => !String.IsNullOrWhiteSpace(Url);

        /// <summary>
        /// Gets the raw URL as saved in Umbraco. The URL may be wrong if referencing content or media that has been
        /// renamed, moved or similar.
        /// </summary>
        [JsonIgnore]
        public string RawUrl { get; }

        #endregion

        #region Constructors

        internal RedirectLinkItem() : base(null) { }

        /// <summary>
        /// Initializes a new link picker item.
        /// </summary>
        /// <param name="id">The ID of the content or media item.</param>
        /// <param name="name">The name (text) of the link.</param>
        /// <param name="url">The URL of the link.</param>
        /// <param name="mode">The mode of the link - either <see cref="RedirectLinkMode.Content"/>,
        /// <see cref="RedirectLinkMode.Media"/> or <see cref="RedirectLinkMode.Url"/>.</param>
        public RedirectLinkItem(int id, string name, string url, RedirectLinkMode mode) : base(null) {
            Id = id;
            Name = name;
            RawUrl = url;
            Mode = mode;
        }

        /// <summary>
        /// Initializes a new link picker item.
        /// </summary>
        /// <param name="id">The ID of the content or media item.</param>
        /// <param name="name">The name (text) of the link.</param>
        /// <param name="url">The URL of the link.</param>
        /// <param name="rawUrl">The raw URL of the link.</param>
        /// <param name="mode">The mode of the link - either <see cref="RedirectLinkMode.Content"/>,
        /// <see cref="RedirectLinkMode.Media"/> or <see cref="RedirectLinkMode.Url"/>.</param>
        public RedirectLinkItem(int id, string name, string url, string rawUrl, RedirectLinkMode mode) : base(null) {
            Id = id;
            Name = name;
            _url = url;
            RawUrl = rawUrl;
            Mode = mode;
        }

        private RedirectLinkItem(JObject obj) : base(obj) {
            Id = obj.GetInt32("id");
            Name = obj.GetString("name");
            RawUrl = obj.GetString("url");
            Mode = obj.GetEnum("mode", RedirectLinkMode.Url);
        }

        #endregion

        #region Member methods

        /// <summary>
        /// Method for calculating the current destination URL of the redirect if the destination is either a content item or media item.
        /// </summary>
        /// <returns>The calculated URL.</returns>
        protected virtual string GetCalculatedUrl() {

            // If we dont have a valid UmbracoContext (eg. during Examine indexing), we simply return the raw URL
            if (UmbracoContext.Current == null) return RawUrl;
            
            // Look up the actual URL for content and media
            switch (Mode) {
                case RedirectLinkMode.Content: {
                    IPublishedContent content = UmbracoContext.Current.ContentCache.GetById(Id);
                    return content == null ? RawUrl : content.Url;
                }
                case RedirectLinkMode.Media: {
                    IPublishedContent media = UmbracoContext.Current.MediaCache.GetById(Id);
                    return media == null ? RawUrl : media.Url;
                }
            }
            
            // Use the raw URL as a fallback
            return RawUrl;
        
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses the specified <paramref name="obj"/> into an instance of <see cref="RedirectLinkItem"/>.
        /// </summary>
        /// <param name="obj">The instance of <see cref="JObject"/> to be parsed.</param>
        public static RedirectLinkItem Parse(JObject obj) {
            return obj == null ? null : new RedirectLinkItem(obj);
        }

        /// <summary>
        /// Initializes a new link item from the specified <paramref name="content"/> representing a content
        /// item.
        /// </summary>
        /// <param name="content">An instance of <see cref="IPublishedContent"/> representing a content item.</param>
        /// <returns>The created <see cref="RedirectLinkItem"/> instance.</returns>
        public static RedirectLinkItem GetFromContent(IPublishedContent content) {
            if (content == null) throw new ArgumentNullException(nameof(content));
            return new RedirectLinkItem(content.Id, content.Name, content.Url, content.Url, RedirectLinkMode.Content);
        }

        /// <summary>
        /// Initializes a new link item from the specified <paramref name="media"/> representing a media item.
        /// </summary>
        /// <param name="media">An instance of <see cref="IPublishedContent"/> representing a media item.</param>
        /// <returns>The created <see cref="RedirectLinkItem"/> instance.</returns>
        public static RedirectLinkItem GetFromMedia(IPublishedContent media) {
            if (media == null) throw new ArgumentNullException(nameof(media));
            return new RedirectLinkItem(media.Id, media.Name, media.Url, media.Url, RedirectLinkMode.Media);
        }

        /// <summary>
        /// Initializes a new link picker item from the specified <paramref name="url"/> and <paramref name="name"/>.
        /// </summary>
        /// <param name="url">The URL of the link.</param>
        /// <param name="name">The name (text) of the link.</param>
        /// <returns>The created <see cref="RedirectLinkItem"/> instance.</returns>
        public static RedirectLinkItem GetFromUrl(string url, string name = null) {
            if (String.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            return new RedirectLinkItem(0, name ?? "", url, RedirectLinkMode.Url);
        }

        #endregion

    }

}