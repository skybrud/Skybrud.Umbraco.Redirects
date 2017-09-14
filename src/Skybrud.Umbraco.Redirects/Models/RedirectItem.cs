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
        internal RedirectItemRow Row { get; private set; }

        [JsonProperty("id")]
        public int Id {
            get { return Row.Id; }
        }

        [JsonProperty("uniqueId")]
        public string UniqueId {
            get { return Row.UniqueId; }
        }

        [JsonProperty("rootNodeId")]
        public int RootNodeId {
            get { return Row.RootNodeId; }
            set { Row.RootNodeId = value; _rootNode = null; _rootNodeDomains = null; }
        }

        [JsonProperty("rootNodeName")]
        public string RootNodeName {
            get {
                if (RootNodeId > 0 && _rootNode == null) _rootNode = ApplicationContext.Current.Services.ContentService.GetById(RootNodeId);
                return _rootNode == null ? null : _rootNode.Name;
            }
        }

        [JsonProperty("rootNodeIcon")]
        public string RootNodeIcon {
            get {
                if (RootNodeId > 0 && _rootNode == null) _rootNode = ApplicationContext.Current.Services.ContentService.GetById(RootNodeId);
                return _rootNode == null ? null : _rootNode.ContentType.Icon;
            }
        }

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
        /// Gets the inbound URL (path) of the redirect. The value value will not contain the domain or the query
        /// string.
        /// </summary>
        [JsonProperty("url")]
        public string Url {
            get { return Row.Url; }
            set { Row.Url = value; }
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
        /// Gets the inbound query string of the redirect.
        /// </summary>
        [JsonProperty("queryString")]
        public string QueryString {
            get { return Row.QueryString; }
            set { Row.QueryString = value; }
        }

        [JsonProperty("linkMode")]
        public RedirectLinkMode LinkMode {
            get { return _linkMode; }
            set { _linkMode = value; Row.LinkMode = _linkMode.ToString().ToLower(); }
        }

        [JsonProperty("linkId")]
        public int LinkId {
            get { return Row.LinkId; }
            set { Row.LinkId = value; }
        }

        [JsonProperty("linkUrl")]
        public string LinkUrl {
            get { return Row.LinkUrl; }
            set { Row.LinkUrl = value; }
        }

        [JsonProperty("linkName")]
        public string LinkName {
            get { return Row.LinkName; }
            set { Row.LinkName = value; }
        }

        [JsonProperty("link")]
        public RedirectLinkItem Link {
            get { return new RedirectLinkItem(LinkId, LinkName, LinkUrl, LinkMode); }
            set {
                if (value == null) throw new ArgumentNullException("value");
                LinkMode = value.Mode;
                LinkId = value.Id;
                LinkUrl = value.Url;
                LinkName = value.Name;
            }
        }

        [JsonProperty("created")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public EssentialsDateTime Created {
            get { return _created; }
            set { _created = value ?? EssentialsDateTime.Zero; Row.Created = _created.UnixTimestamp; }
        }

        [JsonProperty("updated")]
        [JsonConverter(typeof(UnixTimeConverter))]
        public EssentialsDateTime Updated {
            get { return _updated; }
            set { _updated = value ?? EssentialsDateTime.Zero; Row.Updated = _updated.UnixTimestamp; }
		}

		[JsonProperty("permanent")]
		public bool IsPermanent
		{
			get { return Row.IsPermanent; }
			set { Row.IsPermanent = value; }
		}

		[JsonProperty("regex")]
		public bool IsRegex
		{
			get { return Row.IsRegex; }
			set { Row.IsRegex = value; }
		}

		[JsonProperty("forward")]
		public bool ForwardQueryString {
			get { return Row.ForwardQueryString; }
			set { Row.ForwardQueryString = value; }
		}

		#endregion

		#region Constructors

		internal RedirectItem(RedirectItemRow row) {
            _created = EssentialsDateTime.FromUnixTimestamp(row.Created);
            _updated = EssentialsDateTime.FromUnixTimestamp(row.Updated);
            _linkMode = EnumUtils.ParseEnum(row.LinkMode, RedirectLinkMode.Content);
            Row = row;
        }

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