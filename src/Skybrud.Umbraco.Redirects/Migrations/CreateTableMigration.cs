using Skybrud.Umbraco.Redirects.Models.Schema;
using Umbraco.Cms.Infrastructure.Migrations;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Migrations {

    public class CreateTableMigration : MigrationBase {

        public CreateTableMigration(IMigrationContext context) : base(context) { }

        protected override void Migrate() {
            if (TableExists(RedirectSchema.TableName)) return;
            Create.Table<RedirectSchema>().Do();
        }

    }

}