using System.Threading.Tasks;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {

    internal class DummyMigration : AsyncMigrationBase {

        // Dummy migration class because I messed up the migration plan (╯°□°)╯︵ ┻━┻

        public DummyMigration(IMigrationContext context) : base(context) { }

        protected override Task MigrateAsync() {
            return Task.CompletedTask;
        }

    }

}