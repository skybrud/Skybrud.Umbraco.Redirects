//using System;
//using NPoco;
//using Umbraco.Core.Persistence.DatabaseAnnotations;

//namespace Skybrud.Umbraco.Redirects.Models.Database {

//    /// <summary>
//    /// Class representing the database row of a redirect.
//    /// </summary>
//    [TableName(TableName)]
//    [PrimaryKey(PrimaryKey, AutoIncrement = true)]
//    [ExplicitColumns]
//    public class RedirectCustomDomainRow {

//        #region Constants

//        /// <summary>
//        /// Gets the name of the table used in the database.
//        /// </summary>
//        public const string TableName = "SkybrudDomains";

//        /// <summary>
//        /// Gets the primary key of the redirects table.
//        /// </summary>
//        public const string PrimaryKey = nameof(Id);

//        #endregion

//        #region Properties

//        /// <summary>
//        /// Gets or sets the numeric ID (primary key) of the redirect.
//        /// </summary>
//        [Column(PrimaryKey)]
//        [PrimaryKeyColumn(AutoIncrement = true)]
//        public int Id { get; set; }

//        /// <summary>
//        /// Gets or sets the unique ID of the redirect.
//        /// </summary>
//        [Column("UniqueId")]
//        public string UniqueId { get; set; }

//        /// <summary>
//        /// Gets or sets the root node ID of the redirect.
//        /// </summary>
//        [Column("Domain")]
//        public string Domain { get; set; }
        
//        /// <summary>
//        /// Gets or sets the timestamp for when the redirect was created.
//        /// </summary>
//        [Column("Created")]
//        public DateTime Created { get; set; }

//        /// <summary>
//        /// Gets or sets the timestamp for when the redirect was last updated.
//        /// </summary>
//        [Column("Updated")]
//        public DateTime Updated { get; set; }

//		#endregion

//	}

//}