using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models {
    
    public class RedirectDomain {

        #region Properties

        [JsonIgnore]
        public IDomain Domain { get; private set; }

        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("rootNodeId")]
        public int RootNodeId { get; private set; }

        #endregion

        #region Constructors

        private RedirectDomain(IDomain domain) {
            Domain = domain;
            Id = domain.Id;
            Name = domain.DomainName;
            RootNodeId = domain.RootContentId == null ? 0 : domain.RootContentId.Value;
        }

        #endregion

        #region Constructors

        public static RedirectDomain GetFromDomain(IDomain domain) {
            return domain == null ? null : new RedirectDomain(domain);
        }

        #endregion

    }

}