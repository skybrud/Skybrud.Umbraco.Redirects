using Skybrud.Essentials.Strings;

namespace Skybrud.Umbraco.Redirects.Text.Json.Enums {
    
    public class EnumCamelCaseConverter : EnumStringConverter {

        public EnumCamelCaseConverter() : base(TextCasing.CamelCase) { }
            
    }

}