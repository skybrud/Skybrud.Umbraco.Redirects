using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {
    
    internal class DummyMigration  : MigrationBase {

        // Dummy migration class because I messed up the migration plan (╯°□°)╯︵ ┻━┻

        public DummyMigration(IMigrationContext context) : base(context) { }

        protected override void Migrate() { }

    }

}