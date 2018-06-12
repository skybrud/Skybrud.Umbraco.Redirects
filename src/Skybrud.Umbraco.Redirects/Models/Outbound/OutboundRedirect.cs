using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Redirects.Models.Outbound {
    
    /// <summary>
    /// Model for an outbound redirect.
    /// </summary>
    public class OutboundRedirect : JsonObjectBase {

        #region Properties
        
        /// <summary>
        /// Gets whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent")]
        public bool IsPermanent { get; }

        /// <summary>
        /// Gets an instance of <see cref="RedirectLinkItem"/> representing the destination.
        /// </summary>
        [JsonProperty("link")]
        public RedirectLinkItem Link { get; }

        /// <summary>
        /// Gets the URL of the redirects.
        /// </summary>
        [JsonIgnore]
        public string Url => HasLink ? Link.Url : "";

        /// <summary>
        /// Same as <see cref="IsValid"/>.
        /// </summary>
        [JsonIgnore]
        public bool HasLink => IsValid;

        /// <summary>
        /// Gets whether the redirects has a valid link.
        /// </summary>
        [JsonIgnore]
        public bool IsValid => Link != null && Link.IsValid;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance with an empty model.
        /// </summary>
        public OutboundRedirect() : base(null) {
            IsPermanent = true;
            Link = new RedirectLinkItem();
        }

        /// <summary>
        /// Initializes a new instance based on the specified <see cref="JObject"/>.
        /// </summary>
        /// <param name="obj">An instance of <see cref="JObject"/> representing the redirect.</param>
        protected OutboundRedirect(JObject obj) : base(obj) {
            IsPermanent = obj.GetBoolean("permanent");
            Link = obj.GetObject(obj.HasValue("items") ? "items.items[0]" : "link", RedirectLinkItem.Parse) ?? new RedirectLinkItem();
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