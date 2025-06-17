using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations;

internal class NullableColumnsMigration : AsyncMigrationBase {

    private readonly IWebHostEnvironment _webHostEnvironment;

    public NullableColumnsMigration(IMigrationContext context, IWebHostEnvironment webHostEnvironment) : base(context) {
        _webHostEnvironment = webHostEnvironment;
    }

    protected override Task MigrateAsync() {

        try {

            // Save a backup of all redirects
            RedirectsUtils.SaveBackup(_webHostEnvironment, Database);

            Alter
                .Table(RedirectDto.TableName)
                .AlterColumn(nameof(RedirectDto.QueryString)).AsString().Nullable()
                .AlterColumn(nameof(RedirectDto.DestinationQuery)).AsString().Nullable()
                .AlterColumn(nameof(RedirectDto.DestinationFragment)).AsString().Nullable()
                .AlterColumn(nameof(RedirectDto.DestinationCulture)).AsString().Nullable()
                .Do();

            Database.Execute("UPDATE [SkybrudRedirects] SET [QueryString] = null WHERE [QueryString] = '';");
            Database.Execute("UPDATE [SkybrudRedirects] SET [DestinationQuery] = null WHERE [DestinationQuery] = '';");
            Database.Execute("UPDATE [SkybrudRedirects] SET [DestinationFragment] = null WHERE [DestinationFragment] = '';");
            Database.Execute("UPDATE [SkybrudRedirects] SET [DestinationCulture] = null WHERE [DestinationCulture] = '';");

            return Task.CompletedTask;

        } catch (Exception ex) {

            throw new Exception("Failed migration for Skybrud.Umbraco.Redirects. See the Umbraco log for more information.", ex);

        }

    }

}