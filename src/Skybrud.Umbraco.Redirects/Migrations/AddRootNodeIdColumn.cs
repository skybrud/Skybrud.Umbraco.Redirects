using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Core.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {

    internal class AddRootNodeIdColumn : MigrationBase {

        public AddRootNodeIdColumn(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate() {
            if (!TableExists(RedirectItemRow.TableName)) return;
            if (ColumnExists(RedirectItemRow.TableName, "RootNodeId")) return;
            Alter.Table(RedirectItemRow.TableName).AddColumn("RootNodeId").AsInt32().WithDefaultValue(0).Do();
        }
    }

}