using Skybrud.Umbraco.Redirects.Models.Database;
using Umbraco.Core.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations
{

    public class AddLastUsedDate : MigrationBase {

        public AddLastUsedDate(IMigrationContext context) : base(context) { }

        public override void Migrate() {
            if (!TableExists(RedirectItemSchema.TableName)) return;

            if (!ColumnExists(RedirectItemSchema.TableName, "LastUsed"))
            {
                Create.Column("LastUsed")
                    .OnTable(RedirectItemSchema.TableName)
                    .AsDateTime()
                    .Nullable()
                    .Do();
            }
        }
    }
}