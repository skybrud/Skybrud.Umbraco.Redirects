using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Skybrud.Essentials.Time;
using Skybrud.Umbraco.Redirects.Text.Json;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Models.Api {

    public class RedirectModel {

        private readonly IRedirect _redirect;

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id => _redirect.Id;

        [JsonProperty("key")]
        [JsonPropertyName("key")]
        public Guid Key => _redirect.Key;

        [JsonProperty("rootNode")]
        [JsonPropertyName("rootNode")]
        public RedirectRootNodeModel? RootNode { get; }

        [JsonProperty("path")]
        [JsonPropertyName("path")]
        public string Path => _redirect.Path;

        [JsonProperty("queryString")]
        [JsonPropertyName("queryString")]
        public string QueryString => _redirect.QueryString;

        [JsonProperty("url")]
        [JsonPropertyName("url")]
        public string Url => _redirect.Url;

        [JsonProperty("destination")]
        [JsonPropertyName("destination")]
        public RedirectDestinationModel Destination { get; }

        [JsonProperty("createDate")]
        [JsonPropertyName("createDate")]
        [System.Text.Json.Serialization.JsonConverter(typeof(Iso8601TimeConverter))]
        public EssentialsTime CreateDate => _redirect.CreateDate;

        [JsonProperty("updateDate")]
        [JsonPropertyName("updateDate")]
        [System.Text.Json.Serialization.JsonConverter(typeof(Iso8601TimeConverter))]
        public EssentialsTime UpdateDate => _redirect.UpdateDate;

        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public RedirectType Type => _redirect.Type;

        [JsonProperty("permanent")]
        [JsonPropertyName("permanent")]
        public bool IsPermanent => _redirect.IsPermanent;

        [JsonProperty("forward")]
        [JsonPropertyName("forward")]
        public bool ForwardQueryString => _redirect.ForwardQueryString;

        public RedirectModel(IRedirect redirect, RedirectRootNodeModel? rootNode, RedirectDestinationModel destination) {
            _redirect = redirect;
            RootNode = rootNode;
            Destination = destination;
        }

    }

}