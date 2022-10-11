using Skybrud.Umbraco.Redirects.Models.Schema;
using Umbraco.Cms.Infrastructure.Migrations;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Migrations {

    public class RemoveRootIdColumnMigration : RedirectsMigrationBase {

        public RemoveRootIdColumnMigration(IMigrationContext context) : base(context) { }

        protected override void Migrate() {
            DropColumnIfExists(RedirectSchema.TableName, nameof(RedirectSchema.RootId));
        }

    }

}