using System;
using System.Text.Json.Serialization;
using Umbraco.Cms.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models.Api {

    public class RedirectDestinationModel {

        private readonly RedirectDestination _destination;

        public int Id => _destination.Id;

        public Guid Key => _destination.Key;

        public string Url => _destination.Url;

        public string Name { get; }

        public string Icon { get; }

        public RedirectDestinationType Type => _destination.Type;

        [JsonPropertyName("null")]
        public bool IsNull { get; }

        [JsonPropertyName("trashed")]
        public bool IsTrashed { get; }

        [JsonPropertyName("published")]
        public bool IsPublished { get; }

        [JsonPropertyName("warning")]
        public string Warning {
            get {
                if (IsNull) return "deleted";
                if (IsTrashed) return "trashed";
                if (!IsPublished) return "unpublished";
                return null;
            }
        }

        public RedirectDestinationModel(Redirect redirect) : this(redirect.Destination) { }

        public RedirectDestinationModel(RedirectDestination destination) {
            
            _destination = destination;

            switch (Type) {
                
                case RedirectDestinationType.Content:
                    Icon = "icon-article";
                    break;
                
                case RedirectDestinationType.Media:
                    Icon = "icon-picture";
                    IsPublished = true;
                    break;
                
                default:
                    Icon = "icon-link";
                    IsPublished = true;
                    break;

            }

        }

        public RedirectDestinationModel(Redirect redirect, IContent content) {
            _destination = redirect.Destination;
            Name = content?.Name ?? redirect.Destination.Name;
            Icon = content?.ContentType.Icon ?? "icon-article";
            IsNull = content == null;
            IsTrashed = content?.Trashed ?? false;
            IsPublished = content?.Published ?? false;
        }

        public RedirectDestinationModel(Redirect redirect, IMedia media) {
            _destination = redirect.Destination;
            Name = media?.Name ?? redirect.Destination.Name;
            Icon = media?.ContentType.Icon ?? "icon-picture";
            IsNull = media == null;
            IsTrashed = media?.Trashed ?? false;
            IsPublished = !IsNull;
        }

    }

}