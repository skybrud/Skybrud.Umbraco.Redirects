﻿using System;
using Newtonsoft.Json;
using NPoco;
using Skybrud.Essentials.Json.Converters.Time;
using Skybrud.Umbraco.Redirects.Models.Schema;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Skybrud.Umbraco.Redirects.Models.Dtos {

    /// <summary>
    /// Class representing the database row of a redirect.
    /// </summary>
    [TableName(TableName)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class RedirectDto {

        #region Constants

        /// <summary>
        /// Gets the name of the table used in the database.
        /// </summary>
        public const string TableName = RedirectSchema.TableName;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the numeric ID (primary key) of the redirect.
        /// </summary>
        [Column("Id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the redirect.
        /// </summary>
        [Column("Key")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the root node GUID key of the redirect.
        /// </summary>
        [Column("RootKey")]
        public Guid RootKey { get; set; }

        /// <summary>
        /// Gets the inbound path of the redirect. The value value will not contain the domain or the query.
        /// string.
        /// </summary>
        [Column("Url")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the inbound query string of the redirect.
        /// </summary>
        [Column("QueryString")]
        public string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the mode/type of the destination link.
        /// </summary>
        [Column("DestinationType")]
        public string DestinationType { get; set; }

        /// <summary>
        /// Gets or sets the content or media ID of the destination link.
        /// </summary>
        [Column("DestinationId")]
        public int DestinationId { get; set; }

        /// <summary>
        /// Gets or sets the content or media GUID key of the destination link.
        /// </summary>
        [Column("DestinationKey")]
        public Guid DestinationKey { get; set; }

        /// <summary>
        /// Gets or sets the URL of the destination link.
        /// </summary>
        [Column("DestinationUrl")]
        public string DestinationUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL of the destination link.
        /// </summary>
        [Column("DestinationQuery")]
        public string DestinationQuery { get; set; }

        /// <summary>
        /// Gets or sets the URL of the destination link.
        /// </summary>
        [Column("DestinationFragment")]
        public string DestinationFragment { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was created.
        /// </summary>
        [Column("Created")]
        [JsonConverter(typeof(TimeConverter))]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the timestamp for when the redirect was last updated.
        /// </summary>
        [Column("Updated")]
        [JsonConverter(typeof(TimeConverter))]
        public DateTime Updated { get; set; }

        /// <summary>
        /// Gets or sets whether the redirect is permanent.
        /// </summary>
        [Column("IsPermanent")]
        public bool IsPermanent { get; set; }

        /// <summary>
        /// Gets or sets whether the query string should be forwarded.
        /// </summary>
        [Column("ForwardQueryString")]
        public bool ForwardQueryString { get; set; }

        #endregion

    }

}