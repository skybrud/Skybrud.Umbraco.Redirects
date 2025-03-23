using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Umbraco.Cms.Core.Models;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Models.Api;

public class ApiRedirectDestination {

    private readonly IRedirectDestination _destination;

    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public int Id => _destination.Id;

    [JsonProperty("key")]
    [JsonPropertyName("key")]
    public Guid Key => _destination.Key;

    [JsonProperty("url")]
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonProperty("query")]
    [JsonPropertyName("query")]
    public string? Query => _destination.Query;

    [JsonProperty("fragment")]
    [JsonPropertyName("fragment")]
    public string? Fragment => _destination.Fragment;

    [JsonProperty("displayUrl")]
    [JsonPropertyName("displayUrl")]
    public string DisplayUrl => RedirectsUtils.ConcatUrl(Url, Query, Fragment);

    [JsonProperty("name")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonProperty("icon")]
    [JsonPropertyName("icon")]
    public string Icon { get; }

    [JsonProperty("type")]
    [JsonPropertyName("type")]
    public RedirectDestinationType Type => _destination.Type;

    [JsonProperty("null")]
    [JsonPropertyName("null")]
    public bool IsNull { get; }

    [JsonProperty("trashed")]
    [JsonPropertyName("trashed")]
    public bool IsTrashed { get; }

    [JsonProperty("published")]
    [JsonPropertyName("published")]
    public bool IsPublished { get; }

    [JsonProperty("backOfficeUrl")]
    [JsonPropertyName("backOfficeUrl")]
    public string? BackOfficeUrl { get; set; }

    [JsonProperty("warning")]
    [JsonPropertyName("warning")]
    public string? Warning {
        get {
            if (IsNull) return "deleted";
            if (IsTrashed) return "trashed";
            if (!IsPublished) return "unpublished";
            return null;
        }
    }

    [JsonProperty("culture")]
    [JsonPropertyName("culture")]
    public string? Culture { get; set; }

    [JsonProperty("cultureName")]
    [JsonPropertyName("cultureName")]
    public string? CultureName { get; set; }

    public ApiRedirectDestination(IRedirect redirect) : this(redirect.Destination) { }

    public ApiRedirectDestination(IRedirectDestination destination) {

        _destination = destination;

        Name = _destination.Name;
        Url = _destination.Url;

        switch (Type) {

            case RedirectDestinationType.Content:
                Icon = "icon-article";
                BackOfficeUrl = $"/umbraco/section/content/workspace/document/edit/{destination.Key}";
                break;

            case RedirectDestinationType.Media:
                Icon = "icon-picture";
                BackOfficeUrl = $"/umbraco/section/media/workspace/media/edit/{destination.Key}";
                IsPublished = true;
                break;

            default:
                Icon = "icon-link";
                IsPublished = true;
                break;

        }

    }

    public ApiRedirectDestination(IRedirect redirect, IContent? content) {
        _destination = redirect.Destination;
        Name = content?.Name ?? redirect.Destination.Name;
        Url = _destination.Url;
        Icon = content?.ContentType.Icon ?? "icon-article";
        IsNull = content == null;
        IsTrashed = content?.Trashed ?? false;
        IsPublished = content?.Published ?? false;
        BackOfficeUrl = $"/umbraco/#/content/content/edit/{redirect.Destination.Id}";
    }

    public ApiRedirectDestination(IRedirect redirect, IMedia? media) {
        _destination = redirect.Destination;
        Name = media?.Name ?? redirect.Destination.Name;
        Url = _destination.Url;
        Icon = media?.ContentType.Icon ?? "icon-picture";
        IsNull = media == null;
        IsTrashed = media?.Trashed ?? false;
        IsPublished = !IsNull;
        BackOfficeUrl = $"/umbraco/#/media/media/edit/{redirect.Destination.Id}";
    }

}