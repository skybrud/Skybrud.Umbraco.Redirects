using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Extensions;
using Skybrud.Essentials.Strings.Extensions;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Class with information about the destination of a redirect.
    /// </summary>
    public class RedirectDestination : IRedirectDestination {

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
        /// Gets the name of the destination.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the URL of the destination.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets the query string part of the destination.
        /// </summary>
        [JsonProperty("query")]
        public string Query { get; set; }

        /// <summary>
        /// Gets the fragment of the destination - eg. <c>#hello</c>.
        /// </summary>
        [JsonProperty("fragment")]
        public string Fragment { get; set; }

        /// <summary>
        /// Gets the full destination URL.
        /// </summary>
        [JsonProperty("fullUrl")]
        public string FullUrl {
            get {
                StringBuilder sb = new StringBuilder();
                sb.Append(Url);
                if (Query.HasValue()) {
                    sb.Append(Url.Contains("?") ? '&' : '?');
                    sb.Append(Query);
                }
                if (Fragment.HasValue()) {
                    sb.Append(Fragment);
                }
                return sb.ToString();
            }
        }

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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new redirect destination with default values.
        /// </summary>
        public RedirectDestination() {
            Url = string.Empty;
            Name = string.Empty;
            Query = string.Empty;
            Fragment = string.Empty;
        }

        private RedirectDestination(JObject json) {
            Id = json.GetInt32("id");
            Key = json.GetGuid("key");
            Url = json.GetString("url");
            Name = json.GetString("name");
            Query = json.GetString("query") ?? string.Empty;
            Fragment = json.GetString("fragment") ?? string.Empty;
            Type = json.GetEnum("type", RedirectDestinationType.Url);
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Parses the specified <paramref name="json"/> object into an instance of <see cref="RedirectDestination"/>.
        /// </summary>
        /// <param name="json">The JSON object representing the redirect destination.</param>
        /// <returns>An instance of <see cref="RedirectDestination"/>.</returns>
        public static RedirectDestination Parse(JObject json) {
            return json == null ? null : new RedirectDestination(json);
        }

        #endregion

    }

}