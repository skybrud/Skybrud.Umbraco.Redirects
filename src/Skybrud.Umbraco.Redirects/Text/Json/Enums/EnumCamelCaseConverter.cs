using Skybrud.Essentials.Strings;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Text.Json.Enums {

    public class EnumCamelCaseConverter : EnumStringConverter {

        public EnumCamelCaseConverter() : base(TextCasing.CamelCase) { }

    }

}