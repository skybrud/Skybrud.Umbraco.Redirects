using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models.Outbound {

    /// <summary>
    /// Interface describing an outbound redirect.
    /// </summary>
    public interface IOutboundRedirect : IRedirectBase {

        #region Properties

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