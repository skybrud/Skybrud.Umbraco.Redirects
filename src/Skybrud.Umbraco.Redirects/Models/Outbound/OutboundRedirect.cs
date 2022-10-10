using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Redirects.Models.Outbound {

    /// <summary>
    /// Model for an outbound redirect.
    /// </summary>
    public class OutboundRedirect : JsonObjectBase, IOutboundRedirect {

        #region Properties

        /// <summary>
        /// Gets whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent")]
        public bool IsPermanent => Type == RedirectType.Permanent;

        /// <summary>
        /// Gets the type of the redirect - either <see cref="RedirectType.Permanent"/> or <see cref="RedirectType.Temporary"/>.
        /// </summary>
        [JsonProperty("type")]
        public RedirectType Type { get; set; }

        /// <summary>
        /// Gets an instance of <see cref="RedirectDestination"/> representing the destination.
        /// </summary>
        [JsonProperty("destination")]
        public IRedirectDestination Destination { get; set; }

        /// <summary>
        /// Gets whether the query string of the inbound request should be forwarded.
        /// </summary>
        [JsonProperty("forward")]
        public bool ForwardQueryString { get; set; }

        /// <summary>
        /// Same as <see cref="IsValid"/>.
        /// </summary>
        [JsonIgnore]
        public bool HasDestination => IsValid;

        /// <summary>
        /// Gets whether the redirects has a valid link.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => Destination is { IsValid: true };

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance with an empty model.
        /// </summary>
        public OutboundRedirect() : base(null) {
            Destination = new RedirectDestination();
        }

        /// <summary>
        /// Initializes a new instance based on the specified <see cref="JObject"/>.
        /// </summary>
        /// <param name="obj">An instance of <see cref="JObject"/> representing the redirect.</param>
        protected OutboundRedirect(JObject obj) : base(obj) {
            Type = obj.GetBoolean("permanent") ? RedirectType.Permanent : RedirectType.Temporary;
            Destination = obj.GetObject("destination", RedirectDestination.Parse);
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="type"/>, <paramref name="forward"/> and <paramref name="destination"/>.
        /// </summary>
        /// <param name="type">The type of the redirect.</param>
        /// <param name="forward">Whether query string forwarding should be enabled.</param>
        /// <param name="destination">The destination of the redirect.</param>
        public OutboundRedirect(RedirectType type, bool forward, IRedirectDestination destination) : base(null) {
            Type = type;
            ForwardQueryString = forward;
            Destination = destination;
        }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="type"/>, <paramref name="forward"/>, <paramref name="destination"/> and <paramref name="json"/>.
        /// </summary>
        /// <param name="type">The type of the redirect.</param>
        /// <param name="forward">Whether query string forwarding should be enabled.</param>
        /// <param name="destination">The destination of the redirect.</param>
        /// <param name="json">A JSON object representing the redirect.</param>
        public OutboundRedirect(RedirectType type, bool forward, IRedirectDestination destination, JObject json) : base(json) {
            Type = type;
            ForwardQueryString = forward;
            Destination = destination;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses the specified <see cref="JObject"/> into an instance of <see cref="OutboundRedirect"/>.
        /// </summary>
        /// <param name="obj">An instance of <see cref="JObject"/> representing the redirect.</param>
        /// <returns>An instacne of <see cref="OutboundRedirect"/>, or <c>null</c> if <paramref name="obj"/> is <c>null</c>.</returns>
        public static OutboundRedirect Parse(JObject obj) {
            return obj == null ? null : new OutboundRedirect(obj);
        }

        /// <summary>
        /// Deseralizes the specified JSON string into an instance of <see cref="OutboundRedirect"/>.
        /// </summary>
        /// <param name="json">The raw JSON to be parsed.</param>
        public static OutboundRedirect Deserialize(string json) {
            if (json == null) return new OutboundRedirect();
            if (json.StartsWith("{") && json.EndsWith("}")) return JsonUtils.ParseJsonObject(json, Parse);
            return new OutboundRedirect();
        }

        #endregion

    }

}