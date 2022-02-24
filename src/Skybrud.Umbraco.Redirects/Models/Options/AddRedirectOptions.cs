using System;
using Newtonsoft.Json;

namespace Skybrud.Umbraco.Redirects.Models.Options {
    
    /// <summary>
    /// Class with options for adding a redirect.
    /// </summary>
    public class AddRedirectOptions {

        #region Properties
        
        /// <summary>
        /// Gets or set the root node ID of the redirect.
        /// </summary>
        [JsonProperty("rootNodeId")]
        public int RootNodeId { get; set; }
        
        /// <summary>
        /// Gets or set the root node key of the redirect.
        /// </summary>
        [JsonProperty("rootNodeKey")]
        public Guid RootNodeKey { get; set; }
        
        /// <summary>
        /// Gets or set the original URL the redirect.
        /// </summary>
        [JsonProperty("originalurl")]
        public string OriginalUrl { get; set; }
        
        /// <summary>
        /// Gets or set the destination of the redirect.
        /// </summary>
        [JsonProperty("destination")]
        public RedirectDestination Destination { get; set; }
        
        /// <summary>
        /// Gets or set whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent")]
        public bool IsPermanent { get; set; }
        
        /// <summary>
        /// Gets or sets whether query string forwarding should be enabled for the redirect.
        /// </summary>
        [JsonProperty("forward")]
        public bool ForwardQueryString { get; set; }

        #endregion

    }

}