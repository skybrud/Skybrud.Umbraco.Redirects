using Skybrud.Umbraco.Redirects.Models.Database;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;
using Umbraco.Core.Scoping;

namespace Skybrud.Umbraco.Redirects.Migrations {
    
    public class FixRootKeyValue : MigrationBase {
        
        private readonly IScopeProvider _scopeProvider;

        public FixRootKeyValue(IMigrationContext context, IScopeProvider scopeProvider) : base(context) {
            _scopeProvider = scopeProvider;
        }

        public override void Migrate() {
            
            if (TableExists(RedirectItemSchema.TableName) == false) return;

            Database.Execute("UPDATE SkybrudRedirects SET RootKey = (SELECT uniqueId FROM umbracoNode WHERE id = RootId) WHERE RootId > 0 AND RootKey = '00000000-0000-0000-0000-000000000000';");

        }

    }

}