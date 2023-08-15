//using Newtonsoft.Json;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Class representing a domain.
    /// </summary>
    public class RedirectDomain {

        #region Properties

        /// <summary>
        /// Gets a reference to the underlying <see cref="IDomain"/>.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public IDomain Domain { get; }

        /// <summary>
        /// Gets the ID of the domain.
        /// </summary>
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; }

        /// <summary>
        /// Gets the name of the domain.
        /// </summary>
        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the root node ID of the domain.
        /// </summary>
        [JsonProperty("rootNodeId")]
        [JsonPropertyName("rootNodeId")]
        public int RootNodeId { get; }

        #endregion

        #region Constructors

        private RedirectDomain(IDomain domain) {
            Domain = domain;
            Id = domain.Id;
            Name = domain.DomainName;
            RootNodeId = domain.RootContentId ?? 0;
        }

        #endregion

        #region Static methods

        /// <summary>
        /// Initializes a new instance from the specified Umbraco <paramref name="domain"/>.
        /// </summary>
        /// <param name="domain">The Umbraco domain.</param>
        /// <returns>An instance of <see cref="RedirectDomain"/>.</returns>
        [return: NotNullIfNotNull(nameof(domain))]
        public static RedirectDomain? GetFromDomain(IDomain? domain) {
            return domain == null ? null : new RedirectDomain(domain);
        }

        #endregion

    }

}