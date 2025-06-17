using System.Threading.Tasks;
using Skybrud.Umbraco.Redirects.Models.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Migrations;

public class RemoveIsRegexColumnMigration : RedirectsMigrationBase {

    public RemoveIsRegexColumnMigration(IMigrationContext context) : base(context) { }

    protected override Task MigrateAsync() {
        DropColumnIfExists(RedirectSchema.TableName, nameof(RedirectSchema.IsRegex));
        return Task.CompletedTask;
    }

}