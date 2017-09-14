using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Migrations;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Skybrud.Umbraco.Redirects.Migrations {

    [Migration("0.3.0", 1, Package.Alias)]
    public class AddForwardQueryStringColumn : MigrationBase {

        private readonly UmbracoDatabase _database = ApplicationContext.Current.DatabaseContext.Database;
        private readonly DatabaseSchemaHelper _schemaHelper;

        public AddForwardQueryStringColumn(ISqlSyntaxProvider sqlSyntax, ILogger logger) : base(sqlSyntax, logger) {
            _schemaHelper = new DatabaseSchemaHelper(_database, logger, sqlSyntax);
        }

        public override void Up() {
            if (!_schemaHelper.TableExist(RedirectItemRow.TableName)) return;
            Alter.Table(RedirectItemRow.TableName).AddColumn("ForwardQueryString").AsBoolean().WithDefaultValue(false);
        }

        public override void Down() {
            
        }

    }

}