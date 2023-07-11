using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models.Options {

    /// <summary>
    /// Class with options for adding a redirect.
    /// </summary>
    public class AddRedirectOptions {

        #region Properties

        /// <summary>
        /// Gets or sets whether an existing redirect should be overwritten should there already be a redirect with
        /// the same <see cref="RootNodeId"/>, <see cref="RootNodeKey"/> and <see cref="OriginalUrl"/>.
        /// </summary>
        [JsonProperty("overwrite", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("overwrite")]
        public bool Overwrite { get; set; }

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
        public RedirectDestination Destination { get; set; } = null!;

        /// <summary>
        /// Gets or set whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent")]
        [JsonPropertyName("permanent")]
        [Obsolete("Use 'Type' property instead.")]
        public bool IsPermanent {
            get => Type == RedirectType.Permanent;
            set => Type = value ? RedirectType.Permanent : RedirectType.Temporary;
        }

        /// <summary>
        /// Gets or sets the type of the redirect - eg. <see cref="RedirectType.Permanent"/> or <see cref="RedirectType.Temporary"/>.
        /// </summary>
        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public RedirectType Type { get; set; }

        /// <summary>
        /// Gets or sets whether query string forwarding should be enabled for the redirect.
        /// </summary>
        [JsonProperty("forward")]
        [JsonPropertyName("forward")]
        public bool ForwardQueryString { get; set; }

        #endregion

    }

}