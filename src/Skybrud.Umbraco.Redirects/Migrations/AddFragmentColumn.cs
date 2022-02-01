using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Database;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Migrations;

namespace Skybrud.Umbraco.Redirects.Migrations {
    
    internal class AddDestinationColumns : MigrationBase {

        public AddDestinationColumns(IMigrationContext context) : base(context) { }

        public override void Migrate() {

            try {

                // Get the DTOs for all redirects in the database
                List<RedirectItemDto> dtos = Database.Fetch<RedirectItemDto>("SELECT * FROM [SkybrudRedirects];");

                // Map the path to a special redirects folder
                string dir = IOHelper.MapPath("~/App_Data/Skybrud.Umbraco.Redirects");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                // Map the path to the JSON file and save the DTOs to disk as a backup
                string path = Path.Combine(dir, $"Redirects_Backup_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
                JsonUtils.SaveJsonArray(path, JArray.FromObject(dtos));

                // Add the "DestinationQuery" column to the database table
                if (!ColumnExists(RedirectItemDto.TableName, nameof(RedirectItemDto.DestinationQuery))) {
                    Create
                        .Column(nameof(RedirectItemDto.DestinationQuery))
                        .OnTable(RedirectItemDto.TableName)
                        .AsString()
                        .WithDefaultValue(string.Empty)
                        .Do();
                }
                
                // Add the "DestinationFragment" column to the database table
                if (!ColumnExists(RedirectItemDto.TableName, nameof(RedirectItemDto.DestinationFragment))) {
                    Create
                        .Column(nameof(RedirectItemDto.DestinationFragment))
                        .OnTable(RedirectItemDto.TableName)
                        .AsString()
                        .WithDefaultValue(string.Empty)
                        .Do();
                }
                
                // Wrap the DTOs as RedirectItem instances
                List<RedirectItem> redirects = dtos
                    .Select(x => new RedirectItem(x))
                    .ToList();
                
                foreach (RedirectItem redirect in redirects) {
                    
                    // Skip the redirect if we didn't detect any changes
                    if (!RedirectsUtils.NormalizeUrlParts(redirect)) {
                        Logger.Info<AddDestinationColumns>("Redirect with key {Key} is already up-to-date.", redirect.Key);
                        continue;
                    }
                    
                    // Update the redirect in the database
                    Database.Execute(
                        "UPDATE [SkybrudRedirects] SET [DestinationUrl] = @0, [DestinationQuery] = @1, [DestinationFragment] = @2 WHERE [Key] = @3;",
                        redirect.LinkUrl, redirect.LinkQuery, redirect.LinkFragment, redirect.Key
                    );

                    Logger.Info<AddDestinationColumns>("Updated redirect with key {Key}.", redirect.Key);
                    
                }

            } catch (Exception ex) {
            
                throw new Exception("Failed migration for Skybrud.Umbraco.Redirects. See the Umbraco log for more information.", ex);
            
            }

        }

    }

}