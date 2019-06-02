//using Skybrud.Umbraco.Redirects.Models;
//using Skybrud.Umbraco.Redirects.Models.Pocos;
//using Umbraco.Core.Migrations;

//namespace Skybrud.Umbraco.Redirects.Migrations {

//    internal class AddForwardQueryStringColumn : MigrationBase {

//        public AddForwardQueryStringColumn(IMigrationContext context) : base(context)
//        {
//        }

//        public override void Migrate()
//        {
//            if (!TableExists(RedirectItemRow.TableName)) return;
//            if (ColumnExists(RedirectItemRow.TableName, "ForwardQueryString")) return;
//            Alter.Table(RedirectItemRow.TableName).AddColumn("ForwardQueryString").AsBoolean().WithDefaultValue(false).Do();
//        }
//    }



//}