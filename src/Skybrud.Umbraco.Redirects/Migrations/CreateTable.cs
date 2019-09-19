using Skybrud.Umbraco.Redirects.Models.Pocos;
using Umbraco.Core.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {

    public class CreateTable : MigrationBase {

        public CreateTable(IMigrationContext context) : base(context) { }

        public override void Migrate() {
            if (TableExists(RedirectItemSchema.TableName)) return;
            Create.Table<RedirectItemSchema>().Do();
        }

    }

}