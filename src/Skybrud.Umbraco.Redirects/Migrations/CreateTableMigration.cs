using System.Threading.Tasks;
using Skybrud.Umbraco.Redirects.Models.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Migrations;

public class CreateTableMigration : AsyncMigrationBase {

    public CreateTableMigration(IMigrationContext context) : base(context) { }

    protected override Task MigrateAsync() {
        if (TableExists(RedirectSchema.TableName)) return Task.CompletedTask;
        Create.Table<RedirectSchema>().Do();
        return Task.CompletedTask;
    }

}