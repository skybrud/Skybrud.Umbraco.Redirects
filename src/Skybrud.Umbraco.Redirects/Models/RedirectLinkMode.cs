using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Enums;

namespace Skybrud.Umbraco.Redirects.Models {
    
    /// <summary>
    /// Enum describing the type of the link.
    /// </summary>
    [JsonConverter(typeof(EnumLowerCaseConverter))]
    public enum RedirectLinkMode {
    
        /// <summary>
        /// Describes a link that is an external URL.
        /// </summary>
        Url,

        /// <summary>
        /// Describes a link that is a reference to an internal content node in Umbraco.
        /// </summary>
        Content,

        /// <summary>
        /// Describes a link that is a reference to an internal media node in Umbraco.
        /// </summary>
        Media
    
    }

}