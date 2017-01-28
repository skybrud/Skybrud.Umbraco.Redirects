using System;
using Newtonsoft.Json;
using Skybrud.Essentials.Enums;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Essentials.Time;

namespace Skybrud.Umbraco.Redirects.Models {

    public class RedirectItem {

        #region Private fields

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

        [JsonProperty("url")]
        public string Url {
            get { return Row.Url; }
            set { Row.Url = value; }
        }

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
        public bool IsPermanent {
            get { return Row.IsPermanent; }
            set { Row.IsPermanent = value; }
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