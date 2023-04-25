using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

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
        /// Gets the GUID of the root node.
        /// </summary>
        [JsonProperty("key")]
        public Guid Key { get; }

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

        /// <summary>
        /// Gets the path of the root node.
        /// </summary>
        [JsonProperty("path")]
        public IEnumerable<int> Path { get; set; }

        /// <summary>
        /// Gets the domains asscoiated with the root node.
        /// </summary>
        public string[] Domains { get; }

        private RedirectRootNode(IContent content, IEnumerable<RedirectDomain>? domains) {
            Id = content.Id;
            Key = content.Key;
            Name = content.Name!;
            Path = content.Path.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
            Icon = content.ContentType.Icon!;
            Domains = domains?.Select(x => x.Name).ToArray() ?? Array.Empty<string>();
        }

        /// <summary>
        /// Initiaizes a new instance based on the specified <paramref name="content"/> item.
        /// </summary>
        /// <param name="content">The content item representing the root node.</param>
        /// <param name="domains">The domains asscoiated with the root node.</param>
        /// <returns>An instance of <see cref="RedirectRootNode"/>.</returns>
        [return: NotNullIfNotNull(nameof(content))]
        public static RedirectRootNode? GetFromContent(IContent? content, IEnumerable<RedirectDomain>? domains) {
            return content == null ? null : new RedirectRootNode(content, domains);
        }

    }

}