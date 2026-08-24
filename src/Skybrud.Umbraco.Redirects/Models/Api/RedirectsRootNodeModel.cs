using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Models.Api;

public class RedirectsRootNodeModel {

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

    [JsonProperty("backOfficeUrl")]
    [JsonPropertyName("backOfficeUrl")]
    public string? BackOfficeUrl { get; }

    [JsonProperty("domains")]
    [JsonPropertyName("domains")]
    public IReadOnlyList<string> Domains { get; }

    public RedirectsRootNodeModel(IRedirect redirect, IContent? content, string[]? domains) {
        Id = content?.Id ?? 0;
        Key = content?.Key ?? redirect.RootKey;
        Name = content?.Name;
        Icon = content?.ContentType.Icon;
        Domains = domains ?? [];
        BackOfficeUrl = $"/umbraco/section/content/workspace/document/edit/{Key}";
    }

    public RedirectsRootNodeModel(RedirectRootNode rootNode) {
        Id = rootNode.Id;
        Key = rootNode.Key;
        Name = rootNode.Name;
        Icon = rootNode.Icon;
        Domains = rootNode.Domains;
        BackOfficeUrl = $"/umbraco/section/content/workspace/document/edit/{Key}";
    }

}