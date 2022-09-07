using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {

    internal class AddDestinationColumnsMigration : MigrationBase {

        private readonly IWebHostEnvironment _webHostEnvironment;

        public AddDestinationColumnsMigration(IMigrationContext context, IWebHostEnvironment webHostEnvironment) : base(context) {
            _webHostEnvironment = webHostEnvironment;
        }

        protected override void Migrate() {

            try {

                // Save a backup of all redirects
                var result = RedirectsUtils.SaveBackup(_webHostEnvironment, Database);

                // Add the "DestinationQuery" column to the database table
                if (!ColumnExists(RedirectDto.TableName, nameof(RedirectDto.DestinationQuery))) {
                    Create
                        .Column(nameof(RedirectDto.DestinationQuery))
                        .OnTable(RedirectDto.TableName)
                        .AsString()
                        .WithDefaultValue(string.Empty)
                        .Do();
                }

                // Add the "DestinationFragment" column to the database table
                if (!ColumnExists(RedirectDto.TableName, nameof(RedirectDto.DestinationFragment))) {
                    Create
                        .Column(nameof(RedirectDto.DestinationFragment))
                        .OnTable(RedirectDto.TableName)
                        .AsString()
                        .WithDefaultValue(string.Empty)
                        .Do();
                }

                // Wrap the DTOs as RedirectItem instances
                List<Redirect> redirects = result.Dtos
                    .Select(x => new Redirect(x))
                    .ToList();

                foreach (Redirect redirect in redirects) {

                    // Skip the redirect if we didn't detect any changes
                    if (!RedirectsUtils.NormalizeUrlParts(redirect)) {
                        Logger.LogInformation("Redirect with key {Key} is already up-to-date.", redirect.Key);
                        continue;
                    }

                    // Update the redirect in the database
                    Database.Execute(
                        "UPDATE [SkybrudRedirects] SET [DestinationUrl] = @0, [DestinationQuery] = @1, [DestinationFragment] = @2 WHERE [Key] = @3;",
                        redirect.Destination.Url, redirect.Destination.Query, redirect.Destination.Fragment, redirect.Key
                    );

                    Logger.LogInformation("Updated redirect with key {Key}.", redirect.Key);

                }

            } catch (Exception ex) {

                throw new Exception("Failed migration for Skybrud.Umbraco.Redirects. See the Umbraco log for more information.", ex);

            }

        }

    }

}