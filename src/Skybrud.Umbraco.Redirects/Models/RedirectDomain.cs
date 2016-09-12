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

        #endregion

        #region Constructors

        private RedirectDomain(IDomain domain) {
            Domain = domain;
            Id = domain.Id;
            Name = domain.DomainName;
        }

        #endregion

        #region Constructors

        public static RedirectDomain GetFromDomain(IDomain domain) {
            return domain == null ? null : new RedirectDomain(domain);
        }

        #endregion

    }

}