using Skybrud.Umbraco.Redirects.Models.Dtos;
using Skybrud.Umbraco.Redirects.Models.Schema;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {

    public class RemoveIsRegexColumnMigration : MigrationBase {

        public RemoveIsRegexColumnMigration(IMigrationContext context) : base(context) { }

        protected override void Migrate() {
            Database.Execute($"ALTER TABLE {RedirectSchema.TableName} DROP COLUMN IF EXISTS {nameof(RedirectSchema.IsRegex)};");
        }

    }

}