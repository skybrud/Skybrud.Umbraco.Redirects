using System;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Time;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;
using UnixTimeConverter = Skybrud.Umbraco.Redirects.Text.Json.UnixTimeConverter;

namespace Skybrud.Umbraco.Redirects.Models {

    public class Redirect {

        private IContent _rootNode;
        private string[] _rootNodeDomains;
        private EssentialsTime _createDate;
        private EssentialsTime _updateDate;
        private RedirectDestinationType _type;

        #region Properties

        internal RedirectDto Dto { get; }

        /// <summary>
        /// Gets the ID of the redirect.
        /// </summary>
        [JsonProperty("id")]
        public int Id => Dto.Id;

        /// <summary>
        /// Gets the unique ID of the redirect.
        /// </summary>
        [JsonProperty("key")]
        public Guid Key => Dto.Key;

        /// <summary>
        /// Gets or sets the root node ID of the redirect.
        /// </summary>
        [JsonProperty("rootKey")]
        public Guid RootKey {
            get => Dto.RootKey;
            set { Dto.RootKey = value; _rootNode = null; _rootNodeDomains = null; }
        }

        /// <summary>
        /// Gets or sets the inbound URL (path) of the redirect. The value value will not contain the domain or the query string.
        /// </summary>
        [JsonProperty("url")]
        public string Url {
            get => Dto.Url;
            set => Dto.Url = value;
        }

        /// <summary>
        /// Gets or sets the inbound query string of the redirect.
        /// </summary>
        [JsonProperty("queryString")]
        public string QueryString {
            get => Dto.QueryString;
            set => Dto.QueryString = value;
        }

        public RedirectDestination Destination { get; private set; }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was created.
        /// </summary>
        [JsonProperty("createDate")]
        [JsonConverter(typeof(TimeConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(UnixTimeConverter))]
        public EssentialsTime CreateDate {
            get => _createDate;
            set { _createDate = value ?? EssentialsTime.Zero; Dto.Created = _createDate.DateTimeOffset.ToUniversalTime().DateTime; }
        }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was last updated.
        /// </summary>
        [JsonProperty("updateDate")]
        [JsonConverter(typeof(TimeConverter))]
        [System.Text.Json.Serialization.JsonConverter(typeof(UnixTimeConverter))]
        public EssentialsTime UpdateDate {
            get => _updateDate;
            set { _updateDate = value ?? EssentialsTime.Zero; Dto.Updated = _updateDate.DateTimeOffset.ToUniversalTime().DateTime; }
        }

        /// <summary>
        /// Gets or sets whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent")]
        public bool IsPermanent {
            get => Dto.IsPermanent;
            set => Dto.IsPermanent = value;
        }

        /// <summary>
        /// Gets or sets whether the query string should be forwarded.
        /// </summary>
        [JsonProperty("forward")]
        public bool ForwardQueryString {
            get => Dto.ForwardQueryString;
            set => Dto.ForwardQueryString = value;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes an empty redirect.
        /// </summary>
        public Redirect() {
            Dto = new RedirectDto();
            _createDate = EssentialsTime.UtcNow;
            _updateDate = EssentialsTime.UtcNow;
            Dto.Key = Guid.NewGuid();
        }

        internal Redirect(RedirectDto dto) {

            _createDate = new EssentialsTime(dto.Created);
            _updateDate = new EssentialsTime(dto.Updated);

            if (!EnumUtils.TryParseEnum(dto.DestinationType, out RedirectDestinationType type)) {
                throw new RedirectsException($"Unknown redirect type: {dto.DestinationType}");
            }

            Destination = new RedirectDestination {
                Type = type,
                Id = dto.DestinationId,
                Key = dto.DestinationKey,
                Url = dto.DestinationUrl
            };
           
            Dto = dto;

        }

        #endregion

        #region Member methods

        public Redirect SetDestination(RedirectDestination destination) {
            
            Destination = destination ?? throw new ArgumentNullException(nameof(destination));

            Dto.DestinationId = destination.Id;
            Dto.DestinationKey = destination.Key;
            Dto.DestinationType = destination.Type.ToString();
            Dto.DestinationUrl = destination.Url;
            
            return this;

        }

        public Redirect SetDestination(IPublishedContent content) {

            if (content == null) throw new ArgumentNullException(nameof(content));

            switch (content.ItemType) {

                case PublishedItemType.Content:
                    return SetDestination(new RedirectDestination {
                        Id = content.Id,
                        Key = content.Key,
                        Name = content.Name,
                        Url = content.Url(),
                        Type = RedirectDestinationType.Content
                    });

                case PublishedItemType.Media:
                    return SetDestination(new RedirectDestination {
                        Id = content.Id,
                        Key = content.Key,
                        Name = content.Name,
                        Url = content.Url(),
                        Type = RedirectDestinationType.Media
                    });

                default:
                    throw new RedirectsException($"Unsupported item type: {content.ItemType}");

            }

        }

        public Redirect SetDestination(string url) {

            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            
            return SetDestination(new RedirectDestination {
                Url = url,
                Type = RedirectDestinationType.Media
            });

        }

        #endregion

        #region Static methods

        internal static Redirect CreateFromDto(RedirectDto dto) {
            return dto == null ? null : new Redirect(dto);
        }

        #endregion

    }

}