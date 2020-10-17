using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Time;
using Skybrud.Umbraco.Redirects.Models.Database;
using Skybrud.Umbraco.Redirects.Models.Options;
using Umbraco.Core.Models;
using Umbraco.Web.Composing;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Class representing a redirect.
    /// </summary>
    public class RedirectItem {

        #region Private fields

        private IContent _rootNode;
        private string[] _rootNodeDomains;
        private EssentialsTime _created;
        private EssentialsTime _updated;
        private RedirectDestinationType _linkMode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the internal <see cref="RedirectItemDto"/> class used for representing the data as they
        /// are stored in the database.
        /// </summary>
        internal RedirectItemDto Dto { get; }

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
        [JsonProperty("rootId")]
        public int RootId {
            get => Dto.RootId;
            set { Dto.RootId = value; _rootNode = null; _rootNodeDomains = null; }
        }

        /// <summary>
        /// Gets or sets the root node ID of the redirect.
        /// </summary>
        [JsonProperty("rootKey")]
        public Guid RootKey {
            get => Dto.RootKey;
            set { Dto.RootKey = value; _rootNode = null; _rootNodeDomains = null; }
        }

        /// <summary>
        /// Gets the name of the root node, or <c>null</c> if a global redirect.
        /// </summary>
        [JsonProperty("rootNodeName")]
        public string RootNodeName {
            get {
                if (RootId > 0 && _rootNode == null) _rootNode = Current.Services.ContentService.GetById(RootId);
                return _rootNode?.Name;
            }
        }

        /// <summary>
        /// Gets the icon of the root node, or <c>null</c> if a global redirect.
        /// </summary>
        [JsonProperty("rootNodeIcon")]
        public string RootNodeIcon {
            get {
                if (RootId > 0 && _rootNode == null) _rootNode = Current.Services.ContentService.GetById(RootId);
                return _rootNode?.ContentType.Icon;
            }
        }

        /// <summary>
        /// Gets the domains of the root node.
        /// </summary>
        [JsonProperty("rootNodeDomains")]
        public string[] RootNodeDomains {
            get {
                if (RootId > 0 && _rootNodeDomains == null) {
                    _rootNodeDomains = Current.Services.DomainService.GetAssignedDomains(RootId, false).Select(x => x.DomainName).ToArray();
                }
                return _rootNodeDomains ?? new string[0];
            }
        }

        /// <summary>
        /// Gets or sets the inbound URL (path) of the redirect. The value value will not contain the domain or the query
        /// string.
        /// </summary>
        [JsonProperty("url")]
        public string Url {
            get => Dto.Url;
            set => Dto.Url = value;
        }

        /// <summary>
        /// Gets an array of inbound URLs of the redirect. If a root node has been selected for this redirect, the
        /// array will contain a full URL for each domain associated with the root node. The returned URLs will also
        /// contain the query string (if specified).
        /// </summary>
        [JsonProperty("urls")]
        public string[] Urls {
            
            get {

                // Get the URL (path and query string) of the redirect
                string url = Url + (string.IsNullOrWhiteSpace(QueryString) ? string.Empty : "?" + QueryString);

                HttpRequest request = HttpContext.Current == null ? null : HttpContext.Current.Request;

                // Return the full URL for each domain of the selected root node
                if (RootId > 0 && request != null) {

                    List<string> temp = new List<string>();

                    // Calculate the base URL of the current request so we can prioritize it in the list of URLs
                    string baseUrl = (request.IsSecureConnection ? "https://" : "http://") + request.Url.Authority + "/";

                    foreach (string domain in RootNodeDomains) {

                        string prefix = string.Empty;

                        // Prepend the protocol if not already specified
                        if (!domain.StartsWith("http://") && !domain.StartsWith("https://")) {
                            prefix = request.IsSecureConnection ? "https://" : "http://";
                        }

                        // Append the full URL for "domain"
                        temp.Add(prefix + domain.TrimEnd('/') + url);

                    }

                    if (temp.Count > 0) return temp.Where(x => x.StartsWith(baseUrl)).Union(temp.Where(x => !x.StartsWith(baseUrl))).ToArray();

                }

                // Or just return the normal URL as a fallback
                return new[] { url };

            }
        
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
        /// Gets or sets the mode/type of the destination link.
        /// </summary>
        [JsonProperty("linkMode")]
        public RedirectDestinationType LinkMode {
            get => _linkMode;
            set { _linkMode = value; Dto.DestinationType = _linkMode.ToString().ToLower(); }
        }

        /// <summary>
        /// Gets or sets the content or media ID of the destination link.
        /// </summary>
        [JsonProperty("linkId")]
        public int LinkId {
            get => Dto.DestinationId;
            set => Dto.DestinationId = value;
        }

        /// <summary>
        /// Gets or sets the content or media ID of the destination link.
        /// </summary>
        [JsonProperty("linkKey")]
        public Guid LinkKey {
            get => Dto.DestinationKey;
            set => Dto.DestinationKey = value;
        }

        /// <summary>
        /// Gets or sets the URL of the destination link.
        /// </summary>
        [JsonProperty("linkUrl")]
        public string LinkUrl {
            get => Dto.DestinationUrl;
            set => Dto.DestinationUrl = value;
        }

        /// <summary>
        /// Gets or sets an instance of <see cref="RedirectDestination"/> representing the destination link.
        /// </summary>
        [JsonProperty("link")]
        public RedirectDestination Link {
            get => new RedirectDestination(LinkId, LinkKey, LinkUrl, LinkMode);
            set {
                if (value == null) throw new ArgumentNullException(nameof(value));
                LinkMode = value.Type;
                LinkKey = value.Key;
                LinkId = value.Id;
                LinkUrl = value.Url;
            }
        }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was created.
        /// </summary>
        [JsonProperty("created")]
        [JsonConverter(typeof(TimeConverter))]
        public EssentialsTime Created {
            get => _created;
            set { _created = value ?? EssentialsTime.Zero; Dto.Created = _created.DateTimeOffset.ToUniversalTime().DateTime; }
        }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was last updated.
        /// </summary>
        [JsonProperty("updated")]
        [JsonConverter(typeof(TimeConverter))]
        public EssentialsTime Updated {
            get => _updated;
            set { _updated = value ?? EssentialsTime.Zero; Dto.Updated = _updated.DateTimeOffset.ToUniversalTime().DateTime; }
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
        /// Gets or sets whether <see cref="Url"/> is a REGEX pattern.
        /// </summary>
        [JsonProperty("regex")]
		public bool IsRegex {
			get => Dto.IsRegex;
		    set => Dto.IsRegex = value;
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

        internal RedirectItem(AddRedirectOptions options) {

            string url = options.OriginalUrl;
            string query = string.Empty;

            if (options.IsRegex == false) {
                string[] urlParts = url.Split('?');
                url = urlParts[0].TrimEnd('/');
                query = urlParts.Length == 2 ? urlParts[1] : string.Empty;
            }

            RootId = options.RootNodeId;

            LinkId = options.Destination.Id;
            LinkKey = options.Destination.Key;
            LinkUrl = options.Destination.Url;
            LinkMode = options.Destination.Type;

            Url = url;
            QueryString = query;

            Created = EssentialsTime.UtcNow;
            Updated = EssentialsTime.UtcNow;

            IsPermanent = options.IsPermanent;
            IsRegex = options.IsRegex;
            ForwardQueryString = options.ForwardQueryString;

        }

		internal RedirectItem(RedirectItemDto dto) {
            _created = new EssentialsTime(dto.Created);
            _updated = new EssentialsTime(dto.Updated);
            _linkMode = EnumUtils.ParseEnum(dto.DestinationType, RedirectDestinationType.Content);
            Dto = dto;
        }

        /// <summary>
        /// Initializes an empty redirect.
        /// </summary>
        public RedirectItem() {
            Dto = new RedirectItemDto();
            _created = EssentialsTime.UtcNow;
            _updated = EssentialsTime.UtcNow;
            Dto.Key = Guid.NewGuid();
        }

        #endregion

        #region Static methods

        internal static RedirectItem GetFromRow(RedirectItemDto dto) {
            return dto == null ? null : new RedirectItem(dto);
        }

        #endregion

    }

}