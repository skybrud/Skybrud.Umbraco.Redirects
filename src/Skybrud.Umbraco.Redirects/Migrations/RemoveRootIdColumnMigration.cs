using System.Threading.Tasks;
using Skybrud.Umbraco.Redirects.Models.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Migrations;

public class RemoveRootIdColumnMigration : RedirectsMigrationBase {

    public RemoveRootIdColumnMigration(IMigrationContext context) : base(context) { }

    protected override Task MigrateAsync() {
        DropColumnIfExists(RedirectSchema.TableName, nameof(RedirectSchema.RootId));
        return Task.CompletedTask;
    }

}