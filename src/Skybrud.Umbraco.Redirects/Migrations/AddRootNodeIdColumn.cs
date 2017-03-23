using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Skybrud.Umbraco.Redirects.Migrations {

    [Migration("0.2.5", 1, Package.Alias)]
    public class AddRootNodeIdColumn : MigrationBase {

        private readonly UmbracoDatabase _database = ApplicationContext.Current.DatabaseContext.Database;
        private readonly DatabaseSchemaHelper _schemaHelper;

        public AddRootNodeIdColumn(ISqlSyntaxProvider sqlSyntax, ILogger logger) : base(sqlSyntax, logger) {
            _schemaHelper = new DatabaseSchemaHelper(_database, logger, sqlSyntax);
        }

        public override void Up() {
            if (!_schemaHelper.TableExist(RedirectItemRow.TableName)) return;
            Alter.Table(RedirectItemRow.TableName).AddColumn("RootNodeId").AsInt32().WithDefaultValue(0);
        }

        public override void Down() {
            
        }

    }

}