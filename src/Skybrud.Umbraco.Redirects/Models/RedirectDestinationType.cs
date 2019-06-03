using Newtonsoft.Json;
using Skybrud.Essentials.Json.Converters.Enums;
using Skybrud.Essentials.Strings;

namespace Skybrud.Umbraco.Redirects.Models {
    
    /// <summary>
    /// Enum describing the type of the link.
    /// </summary>
    [JsonConverter(typeof(EnumStringConverter), TextCasing.CamelCase)]
    public enum RedirectDestinationType {
    
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