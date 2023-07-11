using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Cms.Core.Models;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Models.Api {

    public class RedirectRootNodeModel {

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonProperty("key")]
        [JsonPropertyName("key")]
        public Guid Key { get; }

        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string? Name { get; }

        [JsonProperty("icon")]
        [JsonPropertyName("icon")]
        public string? Icon { get; }

        [JsonProperty("backofficeUrl")]
        [JsonPropertyName("backofficeUrl")]
        public string? BackOfficeUrl { get; }

        [JsonProperty("domains")]
        [JsonPropertyName("domains")]
        public string[] Domains { get; }

        public RedirectRootNodeModel(IRedirect redirect, IContent? content, string[]? domains, string backOfficeBaseUrl) {
            Id = content?.Id ?? 0;
            Key = content?.Key ?? redirect.RootKey;
            Name = content?.Name;
            Icon = content?.ContentType.Icon;
            Domains = domains ?? Array.Empty<string>();
            BackOfficeUrl = $"{backOfficeBaseUrl}/#/content/content/edit/{Id}";
        }

        public RedirectRootNodeModel(IRedirect redirect, IContent? content, string[]? domains, RedirectsBackOfficeHelper backOffice) {
            Id = content?.Id ?? 0;
            Key = content?.Key ?? redirect.RootKey;
            Name = content?.Name;
            Icon = content?.ContentType.Icon;
            Domains = domains ?? Array.Empty<string>();
            BackOfficeUrl = $"{backOffice.BackOfficeUrl}/#/content/content/edit/{Id}";
        }

        public RedirectRootNodeModel(RedirectRootNode rootNode, string backOfficeBaseUrl) {
            Id = rootNode.Id;
            Key = rootNode.Key;
            Name = rootNode.Name;
            Icon = rootNode.Icon;
            Domains = rootNode.Domains;
            BackOfficeUrl = $"{backOfficeBaseUrl}/#/content/content/edit/{Id}";
        }

        public RedirectRootNodeModel(RedirectRootNode rootNode, RedirectsBackOfficeHelper backOffice) {
            Id = rootNode.Id;
            Key = rootNode.Key;
            Name = rootNode.Name;
            Icon = rootNode.Icon;
            Domains = rootNode.Domains;
            BackOfficeUrl = $"{backOffice.BackOfficeUrl}/#/content/content/edit/{Id}";
        }

    }

}