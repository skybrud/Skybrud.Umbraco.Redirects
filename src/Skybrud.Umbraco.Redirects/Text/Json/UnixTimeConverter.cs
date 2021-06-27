using Skybrud.Essentials.Time;

namespace Skybrud.Umbraco.Redirects.Text.Json {

    public class UnixTimeConverter : TimeConverter {

        public UnixTimeConverter() : base(TimeFormat.UnixTime) { }

    }

}