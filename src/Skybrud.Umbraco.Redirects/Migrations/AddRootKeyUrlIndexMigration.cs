using System;
using System.Threading.Tasks;
using Skybrud.Umbraco.Redirects.Models.Schemas;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations;

internal class AddRootKeyUrlIndexMigration : AsyncMigrationBase {

    internal const string IndexName = $"IX_{RedirectSchema.TableName}_{nameof(RedirectSchema.RootKey)}_{nameof(RedirectSchema.Url)}";

    public AddRootKeyUrlIndexMigration(IMigrationContext context) : base(context) { }

    protected override Task MigrateAsync() {

        try {

            // Add an index for looking up redirects by their root key and inbound path
            if (!IndexExists(IndexName)) {
                Create
                    .Index(IndexName)
                    .OnTable(RedirectSchema.TableName)
                    .OnColumn(nameof(RedirectSchema.RootKey)).Ascending()
                    .OnColumn(nameof(RedirectSchema.Url)).Ascending()
                    .WithOptions().NonClustered()
                    .Do();
            }

            return Task.CompletedTask;

        } catch (Exception ex) {

            throw new Exception("Failed migration for Skybrud.Umbraco.Redirects. See the Umbraco log for more information.", ex);

        }

    }

}