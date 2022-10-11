using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Skybrud.Essentials.Strings;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Text.Json.Enums {

    public class EnumStringConverter : JsonConverterFactory {

        private readonly TextCasing _casing;

        public EnumStringConverter() {
            _casing = TextCasing.PascalCase;
        }

        public EnumStringConverter(TextCasing casing) {
            _casing = casing;
        }

        public sealed override bool CanConvert(Type typeToConvert) {
            return typeToConvert.IsEnum;
        }

        /// <inheritdoc />
        public sealed override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
            var type = typeof(EnumStringConverter<>).MakeGenericType(typeToConvert);
            return (JsonConverter) Activator.CreateInstance(type, new object[] { _casing });

        }

    }

}