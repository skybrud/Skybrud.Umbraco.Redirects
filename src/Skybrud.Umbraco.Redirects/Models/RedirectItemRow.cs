using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Skybrud.Umbraco.Redirects.Models {

    [TableName(TableName)]
    [PrimaryKey(PrimaryKey, autoIncrement = true)]
    [ExplicitColumns]
    public class RedirectItemRow {

        #region Constants

        public const string TableName = "SkybrudRedirects";

        public const string PrimaryKey = "RedirectId";

        #endregion

        #region Properties

        [Column(PrimaryKey)]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("RedirectUniqueId")]
        public string UniqueId { get; set; }

        [Column("RootNodeId")]
        public int RootNodeId { get; set; }

        [Column("Url")]
        public string Url { get; set; }

        [Column("QueryString")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string QueryString { get; set; }

        [Column("LinkMode")]
        public string LinkMode { get; set; }

        [Column("LinkId")]
        public int LinkId { get; set; }

        [Column("LinkUrl")]
        public string LinkUrl { get; set; }

        [Column("LinkName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string LinkName { get; set; }

        [Column("Created")]
        public long Created { get; set; }

        [Column("Updated")]
        public long Updated { get; set; }

		[Column("IsPermanent")]
		public bool IsPermanent { get; set; }

		[Column("IsRegex")]
		public bool IsRegex { get; set; }

		[Column("ForwardQueryString")]
		public bool ForwardQueryString { get; set; }

		#endregion

	}

}