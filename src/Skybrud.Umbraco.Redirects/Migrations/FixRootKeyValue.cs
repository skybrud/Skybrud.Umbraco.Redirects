using Microsoft.Extensions.Logging;
using Skybrud.Umbraco.Redirects.Models.Schema;
using System;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations
{
    internal class FixRootKeyValue : MigrationBase 
    {
        private readonly IContentService _contentService;
        private readonly ILogger<FixRootKeyValue> _logger;

        public FixRootKeyValue(IMigrationContext context, IContentService contentService, ILogger<FixRootKeyValue> logger) : base(context)
        {
            _contentService = contentService;
            _logger = logger;
        }

        protected override void Migrate()
        {

            if (TableExists(RedirectSchema.TableName) == false) return;

            var rows = Database.Fetch<Row>("SELECT * FROM [SkybrudRedirects] WHERE [RootId] > 0 AND [RootKey] = '00000000-0000-0000-0000-000000000000';");

            foreach (var row in rows)
            {

                // Get the content node
                var content = _contentService.GetById(row.RootId);

                // Write to the log if the root node wasn't found
                if (content == null)
                {
                    _logger.LogError("Root node with ID {RootId} could not be found for redirect with key {Key}.", row.RootId, row.Key);
                    continue;
                }

                // Update the redirect in the database
                Database.Execute("UPDATE [SkybrudRedirects] SET [RootKey] = @0 WHERE [Key] = @1;", content.Key, row.Key);

            }

        }

        public class Row
        {

            public Guid Key { get; set; }

            public int RootId { get; set; }

        }
    }
}
