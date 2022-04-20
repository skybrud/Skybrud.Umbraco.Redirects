using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Time;
using Skybrud.Umbraco.Redirects.Exceptions;
using Skybrud.Umbraco.Redirects.Extensions;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Skybrud.Umbraco.Redirects.Text.Json;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Class representing a redirect.
    /// </summary>
    public class Redirect : IRedirect {

        private IRedirectDestination _destination;
        private EssentialsTime _createDate;
        private EssentialsTime _updateDate;

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
        /// Gets or sets the root node key of the redirect.
        /// </summary>
        [JsonProperty("rootKey")]
        public Guid RootKey {
            get => Dto.RootKey;
            set => Dto.RootKey = value;
        }

        /// <summary>
        /// Gets or sets the inbound path of the redirect. The value value will not contain the domain or the query string.
        /// </summary>
        [JsonProperty("path")]
        public string Path {
            get => Dto.Path;
            set => Dto.Path = value;
        }

        /// <summary>
        /// Gets or sets the inbound query string of the redirect.
        /// </summary>
        [JsonProperty("queryString")]
        public string QueryString {
            get => Dto.QueryString;
            set => Dto.QueryString = value;
        }

        /// <summary>
        /// Gets or sets the inbound URL of the redirect.
        /// </summary>
        [JsonProperty("url")]
        public string Url {
            
            get => Dto.Path + (string.IsNullOrWhiteSpace(Dto.QueryString) ? null : "?" + QueryString);

            set  {

                if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

                // Remove the fragment
                value.Split('#', out value);

                // Split the path and query
                value.Split('?', out string path, out string query);

                // Update the DTO properties
                Dto.Path = path.TrimEnd('/');
                Dto.QueryString = query;

            }

        }

        /// <summary>
        /// Gets or sets the destination of the redirect.
        /// </summary>
        public IRedirectDestination Destination {

            get => _destination;

            set {
                _destination = value ?? throw new ArgumentNullException(nameof(value));
                Dto.DestinationId = value.Id;
                Dto.DestinationKey = value.Key;
                Dto.DestinationType = value.Type.ToString();
                Dto.DestinationUrl = value.Url;
                Dto.DestinationQuery = value.Query ?? string.Empty;
                Dto.DestinationFragment = value.Fragment ?? string.Empty;
            }

        }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was created.
        /// </summary>
        [JsonProperty("createDate")]
        [System.Text.Json.Serialization.JsonConverter(typeof(Iso8601TimeConverter))]
        public EssentialsTime CreateDate {
            get => _createDate;
            set { _createDate = value ?? EssentialsTime.Zero; Dto.Created = _createDate.DateTimeOffset.ToUniversalTime().DateTime; }
        }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was last updated.
        /// </summary>
        [JsonProperty("updateDate")]
        [System.Text.Json.Serialization.JsonConverter(typeof(Iso8601TimeConverter))]
        public EssentialsTime UpdateDate {
            get => _updateDate;
            set { _updateDate = value ?? EssentialsTime.Zero; Dto.Updated = _updateDate.DateTimeOffset.ToUniversalTime().DateTime; }
        }

        /// <summary>
        /// Gets or sets the type of the redirect. Possible values are <see cref="RedirectType.Permanent"/> and <see cref="RedirectType.Temporary"/>.
        /// </summary>
        [JsonProperty("type")]
        [JsonPropertyName("type")]
        public RedirectType Type {
            get => Dto.IsPermanent ? RedirectType.Permanent : RedirectType.Temporary;
            set => Dto.IsPermanent = value == RedirectType.Permanent;
        }

        /// <summary>
        /// Gets or sets whether the redirect is permanent.
        /// </summary>
        [JsonProperty("permanent")]
        [JsonPropertyName("permanent")]
        public bool IsPermanent {
            get => Dto.IsPermanent;
            set => Dto.IsPermanent = value;
        }

        /// <summary>
        /// Gets or sets whether the query string should be forwarded.
        /// </summary>
        [JsonProperty("forward")]
        [JsonPropertyName("forward")]
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
            _destination = new RedirectDestination();
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

            _destination = new RedirectDestination {
                Type = type,
                Id = dto.DestinationId,
                Key = dto.DestinationKey,
                Url = dto.DestinationUrl,
                Query = dto.DestinationQuery,
                Fragment = dto.DestinationFragment
            };
           
            Dto = dto;

        }

        #endregion

        #region Static methods

        internal static Redirect CreateFromDto(RedirectDto dto) {
            return dto == null ? null : new Redirect(dto);
        }

        #endregion

    }

}