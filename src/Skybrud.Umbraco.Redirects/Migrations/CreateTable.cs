using Skybrud.Umbraco.Redirects.Models.Database;
using Umbraco.Core.Migrations;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Migrations {

    public class CreateTable : MigrationBase {

        public CreateTable(IMigrationContext context) : base(context) { }

        public override void Migrate() {
            if (TableExists(RedirectItemSchema.TableName)) return;
            Create.Table<RedirectItemSchema>().Do();
        }

    }

}