using System;
using Microsoft.AspNetCore.Hosting;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {

    internal class AddDestinationCultureColumnMigration : MigrationBase {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public AddDestinationCultureColumnMigration(IMigrationContext context, IWebHostEnvironment webHostEnvironment) : base(context) {
            _webHostEnvironment = webHostEnvironment;
        }

        protected override void Migrate() {

            try {

                // Save a backup of all redirects
                RedirectsUtils.SaveBackup(_webHostEnvironment, Database);

                // Add the "DestinationCulture" column to the database table
                if (!ColumnExists(RedirectDto.TableName, nameof(RedirectDto.DestinationCulture))) {
                    Create
                        .Column(nameof(RedirectDto.DestinationCulture))
                        .OnTable(RedirectDto.TableName)
                        .AsString()
                        .WithDefaultValue(string.Empty)
                        .Do();
                }

            } catch (Exception ex) {

                throw new Exception("Failed migration for Skybrud.Umbraco.Redirects. See the Umbraco log for more information.", ex);

            }

        }

    }

}