using Newtonsoft.Json;
using Umbraco.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models {
    
    /// <summary>
    /// Class representing a root node.
    /// </summary>
    public class RedirectRootNode {

        /// <summary>
        /// Gets the ID of the root node.
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; }

        /// <summary>
        /// Gets the name of the root node.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// Gets the icon of the root node.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; }

        private RedirectRootNode(IContent content) {
            Id = content.Id;
            Name = content.Name;
            Icon = content.ContentType.Icon;
        }

        /// <summary>
        /// Initiaizes a new instance based on the specified <paramref name="content"/> item.
        /// </summary>
        /// <param name="content">The content item representing the root node.</param>
        /// <returns>An instance of <see cref="RedirectRootNode"/>.</returns>
        public static RedirectRootNode GetFromContent(IContent content) {
            return content == null ? null : new RedirectRootNode(content);
        }
    
    }

}