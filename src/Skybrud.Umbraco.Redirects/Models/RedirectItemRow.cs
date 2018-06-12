using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Skybrud.Umbraco.Redirects.Models {

    /// <summary>
    /// Class representing the database row of a redirect.
    /// </summary>
    [TableName(TableName)]
    [PrimaryKey(PrimaryKey, autoIncrement = true)]
    [ExplicitColumns]
    public class RedirectItemRow {

        #region Constants

        /// <summary>
        /// Gets the name of the table used in the database.
        /// </summary>
        public const string TableName = "SkybrudRedirects";

        /// <summary>
        /// Gets the primary key of the redirects table.
        /// </summary>
        public const string PrimaryKey = "RedirectId";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the numeric ID (primary key) of the redirect.
        /// </summary>
        [Column(PrimaryKey)]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the redirect.
        /// </summary>
        [Column("RedirectUniqueId")]
        public string UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the root node ID of the redirect.
        /// </summary>
        [Column("RootNodeId")]
        public int RootNodeId { get; set; }

        /// <summary>
        /// Gets the inbound URL (path) of the redirect. The value value will not contain the domain or the query
        /// string.
        /// </summary>
        [Column("Url")]
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the inbound query string of the redirect.
        /// </summary>
        [Column("QueryString")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the mode/type of the destination link.
        /// </summary>
        [Column("LinkMode")]
        public string LinkMode { get; set; }

        /// <summary>
        /// Gets or sets the content or media ID of the destination link.
        /// </summary>
        [Column("LinkId")]
        public int LinkId { get; set; }

        /// <summary>
        /// Gets or sets the URL of the destination link.
        /// </summary>
        [Column("LinkUrl")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// Gets or sets the name of the destination link.
        /// </summary>
        [Column("LinkName")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string LinkName { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was created.
        /// </summary>
        [Column("Created")]
        public long Created { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was last updated.
        /// </summary>
        [Column("Updated")]
        public long Updated { get; set; }


        /// <summary>
        /// Gets or sets whether the redirect is permanent.
        /// </summary>
        [Column("IsPermanent")]
		public bool IsPermanent { get; set; }

        /// <summary>
        /// Gets or sets whether <see cref="Url"/> is a REGEX pattern.
        /// </summary>
        [Column("IsRegex")]
		public bool IsRegex { get; set; }

        /// <summary>
        /// Gets or sets whether the query string should be forwarded.
        /// </summary>
        [Column("ForwardQueryString")]
		public bool ForwardQueryString { get; set; }

		#endregion

	}

}