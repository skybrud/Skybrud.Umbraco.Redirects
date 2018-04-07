using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models {
    
    public class RedirectDomain {

        #region Properties

        [JsonIgnore]
        public IDomain Domain { get; }

        [JsonProperty("id")]
        public int Id { get; }

        [JsonProperty("name")]
        public string Name { get; }

        [JsonProperty("rootNodeId")]
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

        #region Constructors

        public static RedirectDomain GetFromDomain(IDomain domain) {
            return domain == null ? null : new RedirectDomain(domain);
        }

        #endregion

    }

}