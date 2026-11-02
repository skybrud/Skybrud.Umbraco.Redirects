using Skybrud.Essentials.Time;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Text.Json;

internal class UnixTimeConverter : TimeConverter {

    public UnixTimeConverter() : base(TimeFormat.UnixTimeSeconds) { }

}