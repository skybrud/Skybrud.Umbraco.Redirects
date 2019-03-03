using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Core.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations
{
    public class CreateTable : MigrationBase
    {
        public CreateTable(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate()
        {
            if (!TableExists(RedirectItemRow.TableName))
            {
                // TODO: Should not create table based on the model
                Create.Table<RedirectItemRow>(false).Do();
            }
        }
    }
}
