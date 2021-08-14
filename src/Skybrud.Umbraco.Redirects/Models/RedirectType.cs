using NewtonsoftJsonConverter = Newtonsoft.Json.JsonConverterAttribute;
using MicrosoftJsonConverter = System.Text.Json.Serialization.JsonConverterAttribute;

namespace Skybrud.Umbraco.Redirects.Models {
    
    [NewtonsoftJsonConverter(typeof(Essentials.Json.Converters.Enums.EnumCamelCaseConverter))]
    [MicrosoftJsonConverter(typeof(Text.Json.Enums.EnumCamelCaseConverter))]
    public enum RedirectType {

        Permanent,

        Temporary

    }

}