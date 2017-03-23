using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models {
    
    public class RedirectRootNode {

        [JsonProperty("id")]
        public int Id { get; private set; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("icon")]
        public string Icon { get; private set; }

        private RedirectRootNode(IContent content) {
            Id = content.Id;
            Name = content.Name;
            Icon = content.ContentType.Icon;
        }

        public static RedirectRootNode GetFromContent(IContent content) {
            return content == null ? null : new RedirectRootNode(content);
        }
    
    }

}