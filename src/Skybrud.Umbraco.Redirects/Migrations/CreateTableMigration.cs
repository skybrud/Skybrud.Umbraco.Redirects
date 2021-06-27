using Skybrud.Umbraco.Redirects.Models.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {

    public class CreateTableMigration : MigrationBase {

        public CreateTableMigration(IMigrationContext context) : base(context) { }

        public override void Migrate() {
            if (TableExists(RedirectSchema.TableName)) return;
            Create.Table<RedirectSchema>().Do();
        }

    }

}