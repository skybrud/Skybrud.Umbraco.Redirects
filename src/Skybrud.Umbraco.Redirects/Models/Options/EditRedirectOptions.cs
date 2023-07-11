using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models.Options {

    /// <summary>
    /// Class with options for editing a redirect.
    /// </summary>
    public class EditRedirectOptions {

        #region Properties

        /// <summary>
        /// Gets or sets the numeric ID of the redirect.
        /// </summary>
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the key of the redirect.
        /// </summary>
        [JsonProperty("key")]
        [JsonPropertyName("key")]
        public string? Key { get; set; }

        /// <summary>
        /// Gets or set the root node ID of the redirect.
        /// </summary>
        [JsonProperty("rootNodeId")]
        [JsonPropertyName("rootNodeId")]
        public int RootNodeId { get; set; }

        /// <summary>
        /// Gets or set the root node key of the redirect.
        /// </summary>
        [JsonProperty("rootNodeKey")]
        [JsonPropertyName("rootNodeKey")]
        public Guid RootNodeKey { get; set; }

        /// <summary>
        /// Gets or set the original URL the redirect.
        /// </summary>
        [JsonProperty("originalUrl")]
        [JsonPropertyName("originalUrl")]
        public string? OriginalUrl { get; set; }

        /// <summary>
        /// Gets or set the destination of the redirect.
        /// </summary>
        [JsonProperty("destination")]
        [JsonPropertyName("destination")]
        public RedirectDestination? Destination { get; set; }

        /// <summary>
        /// Gets or set whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent")]
        [JsonPropertyName("permanent")]
        public bool IsPermanent { get; set; }

        /// <summary>
        /// Gets or sets whether query string forwarding should be enabled for the redirect.
        /// </summary>
        [JsonProperty("forward")]
        [JsonPropertyName("forward")]
        public bool ForwardQueryString { get; set; }

        #endregion

    }

}