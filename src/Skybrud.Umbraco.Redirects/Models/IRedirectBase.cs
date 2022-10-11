using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.Models.Outbound;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Interface describing a basic redirect.
    /// </summary>
    /// <seealso cref="IRedirect"/>
    /// <seealso cref="Redirect"/>
    /// <seealso cref="IOutboundRedirect"/>
    /// <seealso cref="OutboundRedirect"/>
    public interface IRedirectBase {

        /// <summary>
        /// Gets or sets whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent", Order = 50)]
        [JsonPropertyName("permanent")]
        public bool IsPermanent => Type == RedirectType.Permanent;

        /// <summary>
        /// Gets or sets the type of the redirect. Possible values are <see cref="RedirectType.Permanent"/> and <see cref="RedirectType.Temporary"/>.
        /// </summary>
        [JsonProperty("type", Order = 51)]
        [JsonPropertyName("type")]
        public RedirectType Type { get; set; }

        /// <summary>
        /// Gets or sets whether the query string should be forwarded.
        /// </summary>
        [JsonProperty("forward", Order = 52)]
        [JsonPropertyName("forward")]
        public bool ForwardQueryString { get; set; }

        /// <summary>
        /// Gets or sets the destination of the redirect.
        /// </summary>
        [JsonProperty("destination", Order = 53)]
        [JsonPropertyName("destination")]
        public IRedirectDestination Destination { get; set; }

    }

}