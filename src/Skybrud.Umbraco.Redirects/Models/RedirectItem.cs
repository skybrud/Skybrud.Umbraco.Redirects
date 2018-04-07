using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Time;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Class representing a redirect.
    /// </summary>
    public class RedirectItem {

        #region Private fields

        private IContent _rootNode;
        private string[] _rootNodeDomains;
        private EssentialsDateTime _created;
        private EssentialsDateTime _updated;
        private RedirectLinkMode _linkMode;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a reference to the internal <see cref="RedirectItemRow"/> class used for representing the data as they
        /// are stored in the database.
        /// </summary>
        internal RedirectItemRow Row { get; }

        /// <summary>
        /// Gets the ID of the redirect.
        /// </summary>
        [JsonProperty("id")]
        public int Id => Row.Id;

        /// <summary>
        /// Gets the unique ID of the redirect.
        /// </summary>
        [JsonProperty("uniqueId")]
        public string UniqueId => Row.UniqueId;

        /// <summary>
        /// Gets or sets the root node ID of the redirect.
        /// </summary>
        [JsonProperty("rootNodeId")]
        public int RootNodeId {
            get => Row.RootNodeId;
            set { Row.RootNodeId = value; _rootNode = null; _rootNodeDomains = null; }
        }

        /// <summary>
        /// Gets the name of the root node, or <c>null</c> if a global redirect.
        /// </summary>
        [JsonProperty("rootNodeName")]
        public string RootNodeName {
            get {
                if (RootNodeId > 0 && _rootNode == null) _rootNode = ApplicationContext.Current.Services.ContentService.GetById(RootNodeId);
                return _rootNode?.Name;
            }
        }

        /// <summary>
        /// Gets the icon of the root node, or <c>null</c> if a global redirect.
        /// </summary>
        [JsonProperty("rootNodeIcon")]
        public string RootNodeIcon {
            get {
                if (RootNodeId > 0 && _rootNode == null) _rootNode = ApplicationContext.Current.Services.ContentService.GetById(RootNodeId);
                return _rootNode?.ContentType.Icon;
            }
        }

        /// <summary>
        /// Gets the domains of the root node.
        /// </summary>
        [JsonProperty("rootNodeDomains")]
        public string[] RootNodeDomains {
            get {
                if (RootNodeId > 0 && _rootNodeDomains == null) {
                    _rootNodeDomains = ApplicationContext.Current.Services.DomainService.GetAssignedDomains(RootNodeId, false).Select(x => x.DomainName).ToArray();
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
            get => Row.Url;
            set => Row.Url = value;
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
                string url = Url + (String.IsNullOrWhiteSpace(QueryString) ? "" : "?" + QueryString);

                HttpRequest request = HttpContext.Current == null ? null : HttpContext.Current.Request;

                // Return the full URL for each domain of the selected root node
                if (RootNodeId > 0 && request != null) {

                    List<string> temp = new List<string>();

                    // Calculate the base URL of the current request so we can prioritize it in the list of URLs
                    string baseUrl = (request.IsSecureConnection ? "https://" : "http://") + request.Url.Authority + "/";

                    foreach (string domain in RootNodeDomains) {

                        string prefix = "";

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
            get => Row.QueryString;
            set => Row.QueryString = value;
        }

        /// <summary>
        /// Gets or sets the mode/type of the destination link.
        /// </summary>
        [JsonProperty("linkMode")]
        public RedirectLinkMode LinkMode {
            get => _linkMode;
            set { _linkMode = value; Row.LinkMode = _linkMode.ToString().ToLower(); }
        }

        /// <summary>
        /// Gets or sets the content or media ID of the destination link.
        /// </summary>
        [JsonProperty("linkId")]
        public int LinkId {
            get => Row.LinkId;
            set => Row.LinkId = value;
        }

        /// <summary>
        /// Gets or sets the URL of the destination link.
        /// </summary>
        [JsonProperty("linkUrl")]
        public string LinkUrl {
            get => Row.LinkUrl;
            set => Row.LinkUrl = value;
        }

        /// <summary>
        /// Gets or sets the name of the destination link.
        /// </summary>
        [JsonProperty("linkName")]
        public string LinkName {
            get => Row.LinkName;
            set => Row.LinkName = value;
        }

        /// <summary>
        /// Gets or sets an instance of <see cref="RedirectLinkItem"/> representing the destination link.
        /// </summary>
        [JsonProperty("link")]
        public RedirectLinkItem Link {
            get => new RedirectLinkItem(LinkId, LinkName, LinkUrl, LinkMode);
            set {
                if (value == null) throw new ArgumentNullException(nameof(value));
                LinkMode = value.Mode;
                LinkId = value.Id;
                LinkUrl = value.Url;
                LinkName = value.Name;
            }
        }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was created.
        /// </summary>
        [JsonProperty("created")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public EssentialsDateTime Created {
            get => _created;
            set { _created = value ?? EssentialsDateTime.Zero; Row.Created = _created.UnixTimestamp; }
        }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was last updated.
        /// </summary>
        [JsonProperty("updated")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public EssentialsDateTime Updated {
            get => _updated;
            set { _updated = value ?? EssentialsDateTime.Zero; Row.Updated = _updated.UnixTimestamp; }
		}
        
        /// <summary>
        /// Gets or sets whether the redirect is permanent.
        /// </summary>
		[JsonProperty("permanent")]
		public bool IsPermanent {
			get => Row.IsPermanent;
		    set => Row.IsPermanent = value;
		}

        /// <summary>
        /// Gets or sets whether <see cref="Url"/> is a REGEX pattern.
        /// </summary>
        [JsonProperty("regex")]
		public bool IsRegex {
			get => Row.IsRegex;
		    set => Row.IsRegex = value;
		}

        /// <summary>
        /// Gets or sets whether the query string should be forwarded.
        /// </summary>
		[JsonProperty("forward")]
		public bool ForwardQueryString {
			get => Row.ForwardQueryString;
		    set => Row.ForwardQueryString = value;
		}

		#endregion

		#region Constructors

		internal RedirectItem(RedirectItemRow row) {
            _created = EssentialsDateTime.FromUnixTimestamp(row.Created);
            _updated = EssentialsDateTime.FromUnixTimestamp(row.Updated);
            _linkMode = EnumUtils.ParseEnum(row.LinkMode, RedirectLinkMode.Content);
            Row = row;
        }

        /// <summary>
        /// Initializes an empty redirect.
        /// </summary>
        public RedirectItem() {
            Row = new RedirectItemRow();
            _created = EssentialsDateTime.Now;
            _updated = EssentialsDateTime.Now;
            Row.UniqueId = Guid.NewGuid().ToString();
        }

        #endregion

        #region Static methods

        internal static RedirectItem GetFromRow(RedirectItemRow row) {
            return row == null ? null : new RedirectItem(row);
        }

        #endregion

    }

}