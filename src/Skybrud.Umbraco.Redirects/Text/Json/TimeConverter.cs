using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Skybrud.Essentials.Time;
using Skybrud.Essentials.Time.Iso8601;
using Skybrud.Essentials.Time.Rfc2822;
using Skybrud.Essentials.Time.Rfc822;
using Skybrud.Essentials.Time.UnixTime;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Text.Json {

    public class TimeConverter : JsonConverter<object> {

        /// <summary>
        /// The format to be used when serializing to JSON. Default is <see cref="TimeFormat.Iso8601"/>.
        /// </summary>
        public TimeFormat Format { get; protected set; }

        /// <summary>
        /// Initializes a new converter with default options.
        /// </summary>
        public TimeConverter() {
            Format = TimeFormat.Iso8601;
        }

        /// <summary>
        /// Initializes a new converter for the specified <paramref name="format"/>.
        /// </summary>
        /// <param name="format">The format to be used when serializing to JSON.</param>
        public TimeConverter(TimeFormat format) {
            Format = format;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {

            switch (reader.TokenType) {

                case JsonTokenType.Null:
                    return null;

                case JsonTokenType.Number:
                    return EssentialsTime.FromUnixTimeSeconds(reader.GetInt32());

                case JsonTokenType.String:
                    return EssentialsTime.Parse(reader.GetString());

                default:
                    throw new Exception($"Unsupported token type: {reader.TokenType}");

            }

        }

        public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options) {

            if (value == null) {
                writer.WriteNullValue();
                return;
            }

            switch (value) {

                case DateTime dt:
                    WriteValue(writer, ToFormat(dt, Format));
                    break;

                case DateTimeOffset dto:
                    WriteValue(writer, ToFormat(dto, Format));
                    break;

#pragma warning disable 618
                case EssentialsDateTime edt:
                    WriteValue(writer, ToFormat(edt.DateTime, Format));
                    break;
#pragma warning restore 618

                case EssentialsTime et:
                    WriteValue(writer, ToFormat(et.DateTimeOffset, Format));
                    break;

                case EssentialsPartialDate epd:
                    WriteValue(writer, ToFormat(epd, Format));
                    break;

                case EssentialsDate date:
                    WriteValue(writer, ToFormat(date, Format));
                    break;

                default:
                    throw new ArgumentException("Unknown type " + value.GetType(), nameof(value));

            }

        }

        private void WriteValue(Utf8JsonWriter writer, object value) {

            switch (value) {

                case null:
                    writer.WriteNullValue();
                    break;

                case string str:
                    writer.WriteStringValue(str);
                    break;

                case int int32:
                    writer.WriteNumberValue(int32);
                    break;

                case long int64:
                    writer.WriteNumberValue(int64);
                    break;

                default:
                    throw new Exception($"Unsupported type: {value.GetType()}");

            }

        }


        internal static object ToFormat(DateTime value, TimeFormat format) {

            switch (format) {

                case TimeFormat.Iso8601:
                    return Iso8601Utils.ToString(value);

                case TimeFormat.Rfc822:
                    return Rfc822Utils.ToString(value);

                case TimeFormat.Rfc2822:
                    return Rfc2822Utils.ToString(value);

                case TimeFormat.UnixTime:
                    return (long)UnixTimeUtils.ToSeconds(value);

                default:
                    throw new ArgumentException("Unsupported format " + format, nameof(format));

            }

        }

        internal static object ToFormat(EssentialsDate date, TimeFormat format) {

            switch (format) {

                case TimeFormat.Iso8601:
                    return date.Iso8601;

                default:
                    throw new ArgumentException("Unsupported format " + format, nameof(format));

            }

        }

        internal static object ToFormat(DateTimeOffset value, TimeFormat format) {

            switch (format) {

                case TimeFormat.Iso8601:
                    return Iso8601Utils.ToString(value);

                case TimeFormat.Rfc822:
                    return Rfc822Utils.ToString(value);

                case TimeFormat.Rfc2822:
                    return Rfc2822Utils.ToString(value);

                case TimeFormat.UnixTime:
                    return (long) UnixTimeUtils.ToSeconds(value);

                default:
                    throw new ArgumentException("Unsupported format " + format, nameof(format));

            }

        }

        internal static object ToFormat(EssentialsPartialDate value, TimeFormat format) {

            switch (format) {

                case TimeFormat.Iso8601:
                    return value.ToString();

                default:
                    throw new ArgumentException("Unsupported format " + format, nameof(format));

            }

        }
    }

}