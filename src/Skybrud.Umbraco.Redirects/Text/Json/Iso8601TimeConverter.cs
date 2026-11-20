using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Skybrud.Essentials.Time;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Text.Json;

public class Iso8601TimeConverter : JsonConverter<EssentialsTime> {

    public override EssentialsTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return reader.TokenType switch {
            JsonTokenType.Null => null,
            JsonTokenType.String => EssentialsTime.Parse(reader.GetString()!),
            _ => throw new Exception($"Unsupported token type: {reader.TokenType}")
        };
    }

    public override void Write(Utf8JsonWriter writer, EssentialsTime? value, JsonSerializerOptions options) {

        if (value == null) {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStringValue(value.Iso8601);

    }

}