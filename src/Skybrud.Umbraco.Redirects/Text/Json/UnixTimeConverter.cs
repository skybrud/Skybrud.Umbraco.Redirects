using Skybrud.Essentials.Time;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Text.Json {

    public class UnixTimeConverter : TimeConverter {

        public UnixTimeConverter() : base(TimeFormat.UnixTime) { }

    }

}