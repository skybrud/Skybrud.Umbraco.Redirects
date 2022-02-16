using System;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models.ContentEditing;

namespace Skybrud.Umbraco.Redirects.Models {
    
    public interface IRedirectDestination {
        
        /// <summary>
        /// Gets the ID of the selected content or media. If an URL has been selected, this will return <c>0</c>.
        /// </summary>
        [JsonProperty("id")]
        int Id { get; set; }

        /// <summary>
        /// Gets the GUID key of the selected content or media. If an URL has been selected, this will return <c>null</c>.
        /// </summary>
        [JsonProperty("key")]
        Guid Key { get; set; }

        /// <summary>
        /// Gets the name of the destination.
        /// </summary>
        [JsonProperty("name")]
        string Name { get; set; }

        /// <summary>
        /// Gets the URL of the destination.
        /// </summary>
        [JsonProperty("url")]
        string Url { get; set; }

        /// <summary>
        /// Gets the query string part of the destination.
        /// </summary>
        [JsonProperty("query")]
        string Query { get; set; }

        /// <summary>
        /// Gets the fragment of the destination - eg. <c>#hello</c>.
        /// </summary>
        [JsonProperty("fragment")]
        string Fragment { get; set; }

        /// <summary>
        /// Gets the full destination URL.
        /// </summary>
        [JsonProperty("fullUrl")]
        string FullUrl { get; }

        /// <summary>
        /// Gets the type of the destination.
        /// </summary>
        [JsonProperty("type")]
        RedirectDestinationType Type { get; set; }

        /// <summary>
        /// Gets whether the link is valid.
        /// </summary>
        [JsonIgnore]
        bool IsValid { get; }

    }

}