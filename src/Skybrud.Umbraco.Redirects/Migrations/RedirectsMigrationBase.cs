using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {
    
    public abstract class RedirectsMigrationBase : MigrationBase {
        
        protected RedirectsMigrationBase(IMigrationContext context) : base(context) { }
        
        protected void DropColumn(string tableName, string columnName) {
            Database.Execute(string.Format(SqlSyntax.DropColumn, SqlSyntax.GetQuotedTableName(tableName), SqlSyntax.GetQuotedColumnName(columnName)));
        }
        
        protected void DropColumnIfExists(string tableName, string columnName) {
            if (ColumnExists(tableName, columnName)) DropColumn(tableName, columnName);
        }

    }

}
