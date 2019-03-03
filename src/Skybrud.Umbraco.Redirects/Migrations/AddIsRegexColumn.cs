using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Skybrud.Umbraco.Redirects.Migrations {

    internal class AddIsRegexColumn : MigrationBase {

        public AddIsRegexColumn(IMigrationContext context) : base(context)
        {
        }

        public override void Migrate() {
            if (!TableExists(RedirectItemRow.TableName)) return;
            if (ColumnExists(RedirectItemRow.TableName, "IsRegex")) return;
            Alter.Table(RedirectItemRow.TableName).AddColumn("IsRegex").AsBoolean().WithDefaultValue(false).Do();
        }

    }

}