using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.Migrations {
    
    internal class AddDestinationColumnsMigration : MigrationBase {
        
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AddDestinationColumnsMigration(IMigrationContext context, IWebHostEnvironment webHostEnvironment) : base(context) {
            _webHostEnvironment = webHostEnvironment;
        }

        protected override void Migrate() {

            try {

                // Get the DTOs for all redirects in the database
                List<RedirectDto> dtos = Database.Fetch<RedirectDto>("SELECT * FROM [SkybrudRedirects];");

                // Map the path to a special redirects folder
                string dir = _webHostEnvironment.MapPathWebRoot("~/App_Data/Skybrud.Umbraco.Redirects");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // Map the path to the JSON file and save the DTOs to disk as a backup
                string path = Path.Combine(dir, $"Redirects_Backup_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
                JsonUtils.SaveJsonArray(path, JArray.FromObject(dtos));

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
                List<Redirect> redirects = dtos
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