using System;
using Skybrud.Umbraco.Redirects.Models.Database;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Services;

namespace Skybrud.Umbraco.Redirects.Migrations {
    
    internal class FixRootKeyValue : MigrationBase {
        
        private readonly IContentService _contentService;

        public FixRootKeyValue(IMigrationContext context, IContentService contentService) : base(context) {
            _contentService = contentService;
        }

        public override void Migrate() {
            
            if (TableExists(RedirectItemSchema.TableName) == false) return;

            var rows = Database.Fetch<Row>("SELECT * FROM [SkybrudRedirects] WHERE [RootId] > 0 AND [RootKey] = '00000000-0000-0000-0000-000000000000';");
            
            foreach (var row in rows) {

                // Get the content node
                var content = _contentService.GetById(row.RootId);

                // Write to the log if the root node wasn't found
                if (content == null) {
                    Logger.Error<FixRootKeyValue>("Root node with ID {RootId} could not be found for redirect with key {Key}.", row.RootId, row.Key);
                    continue;
                }

                // Update the redirect in the database
                Database.Execute("UPDATE [SkybrudRedirects] SET [RootKey] = @0 WHERE [Key] = @1;", content.Key, row.Key);

            }

        }

        public class Row {

            public Guid Key { get; set; }

            public int RootId { get; set; }

        }

    }

}