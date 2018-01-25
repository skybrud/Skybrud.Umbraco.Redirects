using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Essentials.Json.Extensions;

namespace Skybrud.Umbraco.Redirects.Models.Outbound {
    
    public class OutboundRedirect {

        #region Properties

        [JsonIgnore]
        public JObject JObject { get; }

        [JsonProperty("permanent")]
        public bool IsPermanent { get; }

        [JsonProperty("link")]
        public RedirectLinkItem Link { get; }

        [JsonIgnore]
        public string Url => HasLink ? Link.Url : "";

        [JsonIgnore]
        public bool HasLink => Link != null && Link.IsValid;

        [JsonIgnore]
        public bool IsValid => HasLink;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance with an empty model.
        /// </summary>
        public OutboundRedirect() {
            IsPermanent = true;
            Link = new RedirectLinkItem();
        }

        /// <summary>
        /// Initializes a new instance based on the specified <see cref="JObject"/>.
        /// </summary>
        /// <param name="obj">An instance of <see cref="JObject"/> representing the redirect.</param>
        protected OutboundRedirect(JObject obj) {
            JObject = obj;
            IsPermanent = obj.GetBoolean("permanent");
            Link = obj.GetObject(obj.HasValue("items") ? "items.items[0]" : "link", RedirectLinkItem.Parse) ?? new RedirectLinkItem();
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses the specified <see cref="JObject"/> into an instance of <see cref="OutboundRedirect"/>.
        /// </summary>
        /// <param name="obj">An instance of <see cref="JObject"/> representing the redirect.</param>
        /// <returns>An instacne of <see cref="OutboundRedirect"/>, or <code>null</code> if <code>obj</code> is <code>null</code>.</returns>
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