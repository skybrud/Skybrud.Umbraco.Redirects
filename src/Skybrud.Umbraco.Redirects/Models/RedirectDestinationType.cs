using NewtonsoftJsonConverter = Newtonsoft.Json.JsonConverterAttribute;
using MicrosoftJsonConverter = System.Text.Json.Serialization.JsonConverterAttribute;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Enum describing the type of the link.
    /// </summary>
    [NewtonsoftJsonConverter(typeof(Essentials.Json.Converters.Enums.EnumCamelCaseConverter))]
    [MicrosoftJsonConverter(typeof(Text.Json.Enums.EnumCamelCaseConverter))]
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