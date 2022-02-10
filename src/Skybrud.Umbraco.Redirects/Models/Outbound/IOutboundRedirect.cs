using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models.Outbound {

    /// <summary>
    /// Interface describing an outbound redirect.
    /// </summary>
    public interface IOutboundRedirect {

        #region Properties

        /// <summary>
        /// Gets whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent")]
        bool IsPermanent { get; }

        /// <summary>
        /// Gets the type of the redirect - either <see cref="RedirectType.Permanent"/> or <see cref="RedirectType.Temporary"/>.
        /// </summary>
        [JsonProperty("type")]
        RedirectType Type { get; }

        /// <summary>
        /// Gets an instance of <see cref="RedirectDestination"/> representing the destination.
        /// </summary>
        [JsonProperty("destination")]
        RedirectDestination Destination { get; }

        /// <summary>
        /// Gets whether the query string of the inbound request should be forwarded.
        /// </summary>
        [JsonProperty("forward")]
        bool ForwardQueryString { get; }

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

    }

}