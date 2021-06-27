using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models {
    
    public class RedirectDestination {
        
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
        /// Gets the URL of the destination.
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets the name of the destination.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

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

    }

}