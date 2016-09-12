using System;
using Newtonsoft.Json;
using Skybrud.LinkPicker;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Skybrud.Umbraco.Redirects.Models {

    [TableName(TableName)]
    [PrimaryKey(PrimaryKey, autoIncrement = true)]
    [ExplicitColumns]
    public class RedirectItem {

        #region Constants

        public const string TableName = "SkybrudRedirects";

        public const string PrimaryKey = "RedirectId";

        #endregion

        #region Properties

        [Column(PrimaryKey)]
        [PrimaryKeyColumn(AutoIncrement = true)]
        [JsonProperty("id")]
        public int Id { get; set; }

        [Column("RedirectUniqueId")]
        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        [Column("Url")]
        [JsonProperty("url")]
        public string Url { get; set; }

        [Column("QueryString")]
        [NullSetting(NullSetting = NullSettings.Null)]
        [JsonProperty("queryString")]
        public string QueryString { get; set; }

        [Column("LinkMode")]
        [JsonIgnore]
        public string LinkModeStr {
            get { return LinkMode.ToString().ToLower(); }
            set { LinkMode = (LinkPickerMode)Enum.Parse(typeof(LinkPickerMode), value, true); }
        }

        [Ignore]
        [JsonProperty("linkMode")]
        public LinkPickerMode LinkMode { get; set; }

        [Column("LinkId")]
        [JsonProperty("linkId")]
        public int LinkId { get; set; }

        [Column("LinkUrl")]
        [JsonProperty("linkUrl")]
        public string LinkUrl { get; set; }

        [Column("LinkName")]
        [JsonProperty("linkName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string LinkName { get; set; }

        [Ignore]
        [JsonProperty("link")]
        public LinkPickerItem Link {
            get { return new LinkPickerItem(LinkId, LinkName, LinkUrl, null, LinkMode); }
            set {
                if (value == null) throw new ArgumentNullException("value");
                LinkMode = value.Mode;
                LinkId = value.Id;
                LinkUrl = value.Url;
                LinkName = value.Name;
            }
        }

        [Ignore]
        [JsonIgnore]
        public DateTime Created { get; set; }

        [Column("Created")]
        [JsonProperty("created")]
        public long CreatedUnixTimestamp {
            get { return (long) (Created.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; }
            set { Created = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value); }
        }

        [Ignore]
        [JsonIgnore]
        public DateTime Updated { get; set; }

        [Column("Updated")]
        [JsonProperty("updated")]
        public long UpdatedUnixTimestamp {
            get { return (long)(Updated.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; }
            set { Updated = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value); }
        }

        [Column("IsPermanent")]
        [JsonProperty("permanent")]
        public bool IsPermanent { get; set; }

        #endregion

    }

}