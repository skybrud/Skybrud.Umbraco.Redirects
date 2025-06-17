using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations;

internal class AddDestinationNameColumnMigration : AsyncMigrationBase {

    private readonly IWebHostEnvironment _webHostEnvironment;

    public AddDestinationNameColumnMigration(IMigrationContext context, IWebHostEnvironment webHostEnvironment) : base(context) {
        _webHostEnvironment = webHostEnvironment;
    }

    protected override Task MigrateAsync() {

        try {

            // Save a backup of all redirects
            RedirectsUtils.SaveBackup(_webHostEnvironment, Database);

            // Add the "DestinationCulture" column to the database table
            if (!ColumnExists(RedirectDto.TableName, nameof(RedirectDto.DestinationName))) {
                Create
                    .Column(nameof(RedirectDto.DestinationName))
                    .OnTable(RedirectDto.TableName)
                    .AsString()
                    .Nullable()
                    .Do();
            }

            return Task.CompletedTask;

        } catch (Exception ex) {

            throw new Exception("Failed migration for Skybrud.Umbraco.Redirects. See the Umbraco log for more information.", ex);

        }

    }

}