using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

using MicrosoftJsonConverter = System.Text.Json.Serialization.JsonConverterAttribute;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Models.Api {

    public class RedirectNodeModel {

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonProperty("key")]
        [JsonPropertyName("key")]
        public Guid Key { get; }

        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonProperty("url")]
        [JsonPropertyName("url")]
        public string? Url { get; }

        [JsonProperty("type")]
        [JsonPropertyName("type")]
        [MicrosoftJsonConverter(typeof(Text.Json.Enums.EnumCamelCaseConverter))]
        public RedirectDestinationType Type { get; }

        [JsonProperty("publised")]
        [JsonPropertyName("published")]
        public bool IsPublished { get; }

        [JsonProperty("trashed")]
        [JsonPropertyName("trashed")]
        public bool IsTrashed { get; }

        public RedirectNodeModel(IContent content, IPublishedContent? published) {
            Id = content.Id;
            Key = content.Key;
            Name = content.Name!;
            Type = RedirectDestinationType.Content;
            IsPublished = content.Published;
            IsTrashed = content.Trashed;
            Url = published?.Url();
        }

        public RedirectNodeModel(IMedia media, IPublishedContent? published) {
            Id = media.Id;
            Key = media.Key;
            Name = media.Name!;
            Type = RedirectDestinationType.Media;
            IsPublished = true;
            IsTrashed = media.Trashed;
            Url = published?.Url();
        }

    }

}
