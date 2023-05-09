using System;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Models.Api {

    public class RedirectDestinationModel {

        private readonly IRedirectDestination _destination;

        public int Id => _destination.Id;

        public Guid Key => _destination.Key;

        public string Url { get; set; }

        public string Query => _destination.Query;

        public string Fragment => _destination.Fragment;

        public string DisplayUrl => RedirectsUtils.ConcatUrl(Url, Query, Fragment);

        public string Name { get; set; }

        public string Icon { get; }

        public RedirectDestinationType Type => _destination.Type;

        [JsonPropertyName("null")]
        public bool IsNull { get; }

        [JsonPropertyName("trashed")]
        public bool IsTrashed { get; }

        [JsonPropertyName("published")]
        public bool IsPublished { get; }

        [JsonPropertyName("backOfficeUrl")]
        public string? BackOfficeUrl { get; set; }

        [JsonPropertyName("warning")]
        public string? Warning {
            get {
                if (IsNull) return "deleted";
                if (IsTrashed) return "trashed";
                if (!IsPublished) return "unpublished";
                return null;
            }
        }

        [JsonPropertyName("culture")]
        public string? Culture { get; set; }

        [JsonPropertyName("cultureName")]
        public string? CultureName { get; set; }

        public RedirectDestinationModel(IRedirect redirect) : this(redirect.Destination) { }

        public RedirectDestinationModel(IRedirectDestination destination) {

            _destination = destination;

            Name = _destination.Name;
            Url = _destination.Url;

            switch (Type) {

                case RedirectDestinationType.Content:
                    Icon = "icon-article";
                    BackOfficeUrl = $"/umbraco/#/content/content/edit/{destination.Id}";
                    break;

                case RedirectDestinationType.Media:
                    Icon = "icon-picture";
                    BackOfficeUrl = $"/umbraco/#/media/media/edit/{destination.Id}";
                    IsPublished = true;
                    break;

                default:
                    Icon = "icon-link";
                    IsPublished = true;
                    break;

            }

        }

        public RedirectDestinationModel(IRedirect redirect, IContent? content) {
            _destination = redirect.Destination;
            Name = content?.Name ?? redirect.Destination.Name;
            Url = _destination.Url;
            Icon = content?.ContentType.Icon ?? "icon-article";
            IsNull = content == null;
            IsTrashed = content?.Trashed ?? false;
            IsPublished = content?.Published ?? false;
            BackOfficeUrl = $"/umbraco/#/content/content/edit/{redirect.Destination.Id}";
        }

        public RedirectDestinationModel(IRedirect redirect, IMedia? media) {
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

}