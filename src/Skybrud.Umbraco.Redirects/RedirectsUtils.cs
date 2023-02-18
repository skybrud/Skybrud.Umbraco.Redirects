using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Essentials.Reflection;
using Skybrud.Essentials.Strings.Extensions;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Dtos;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Semver;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.DependencyInjection;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects {

    internal class RedirectsUtils {

        /// <summary>
        /// Returns the concatenated URL based on <paramref name="url"/> and <paramref name="query"/>.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="query">The query string.</param>
        /// <returns>The combined URL.</returns>
        public static string ConcatUrl(string url, string query) {
            return $"{url}{(string.IsNullOrWhiteSpace(query) ? null : "?" + query)}";
        }

        /// <summary>
        /// Returns the concatenated URL based on <paramref name="url"/>, <paramref name="query"/> and <paramref name="fragment"/>.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="query">The query string.</param>
        /// <param name="fragment">The fragment.</param>
        /// <returns>The combined URL.</returns>
        public static string ConcatUrl(string url, string query, string fragment) {
            return $"{url}{(string.IsNullOrWhiteSpace(query) ? null : "?" + query)}{fragment}";
        }

        public static bool NormalizeUrlParts(Redirect redirect) {

            if (redirect.Destination is null) return false;

            string url = redirect.Destination.Url;
            string query = redirect.Destination.Query;
            string? fragment = null;

            // Isolate the fragment if specified in the URL
            int pos1 = url.IndexOf('#');
            if (pos1 >= 0) {
                fragment = url.Substring(pos1);
                url = url.Substring(0, pos1);
            }

            // Isolate the query string if specified in the URL
            int pos2 = url.IndexOf('?');
            if (pos2 >= 0) {
                query += "&" + url.Substring(pos2 + 1);
                url = url.Substring(0, pos2);
            }

            // Parse the "fragment" value
            if (redirect.Destination.Fragment.HasValue()) {

                string temp = redirect.Destination.Fragment;

                // Isolate the fragment if specified in the "anchor" value (overwrites fragment from the URL)
                var pos3 = temp.IndexOf('#');
                if (pos3 >= 0) {
                    fragment = temp.Substring(pos3);
                    temp = pos3 > 0 ? temp.Substring(0, pos3 - 1) : string.Empty;
                }

                // Treat remaining anchor value as query string (append if URL also has query string)
                if (temp.HasValue()) {
                    if (temp.IndexOf('?') == 0 || temp.IndexOf('&') == 0) {
                        query += "&" + temp.Substring(1);
                    } else {
                        query += "&" + temp;
                    }
                }

            }

            string linkUrl = url;
            string linkQuery = query == null ? string.Empty : query.TrimStart('&');
            string linkFragment = fragment ?? string.Empty;

            bool hasChanges = (redirect.Destination.Url != linkUrl || redirect.Destination.Query != linkQuery || redirect.Destination.Fragment != linkFragment);

            redirect.Destination.Url = linkUrl;
            redirect.Destination.Query = linkQuery;
            redirect.Destination.Fragment = linkFragment;

            return hasChanges;

        }

        public class RedirectsBackupResult {

            public string Path { get; }

            public IReadOnlyList<RedirectDto> Dtos { get; }

            public RedirectsBackupResult(string path, IReadOnlyList<RedirectDto> dtos) {
                Path = path;
                Dtos = dtos;
            }

        }

        public static SemVersion GetSemVersion() {
            return SemVersion.Parse(ReflectionUtils.GetInformationalVersion(typeof(RedirectsUtils).Assembly));
        }

        /// <summary>
        /// Saves a backup of all redirects to a new JSON file to the <c>~/umbraco/Data/Skybrud.Umbraco.Redirects</c>
        /// directory. If there are currently not redirects in the database, no redirects will be saved.
        /// </summary>
        public static void SaveBackup() {
            var webHostEnvironment = GetService<IWebHostEnvironment>();
            var database = GetService<IUmbracoDatabase>();
            SaveBackup(webHostEnvironment, database);
        }

        /// <summary>
        /// Saves a backup of all redirects to a new JSON file to the <c>~/umbraco/Data/Skybrud.Umbraco.Redirects</c>
        /// directory. If there are currently not redirects in the database, no redirects will be saved.
        /// </summary>
        /// <param name="webHostEnvironment">A reference to the current <see cref="IWebHostEnvironment"/>.</param>
        /// <param name="database">A reference to the current <see cref="IUmbracoDatabase"/>.</param>
        /// <returns>An instance of <see cref="RedirectsBackupResult"/>.</returns>
        public static RedirectsBackupResult SaveBackup(IWebHostEnvironment webHostEnvironment, IUmbracoDatabase database) {

            // Map the path to a special redirects folder (both old and new)
            string dir1 = webHostEnvironment.MapPathWebRoot($"~/App_Data/{RedirectsPackage.Alias}");
            string dir2 = webHostEnvironment.MapPathContentRoot($"{Constants.SystemDirectories.Data}/{RedirectsPackage.Alias}");

            // Older releases would create a bacup in the "AppData" directory in the web root. Even though the
            // "App_Data" still appear to be protected in ASP.NET Core, we shouldn't save data to the web root. So
            // we've previously created a folder inside "App_Data" in the web root, we move the directory to the
            // correct location.
            if (Directory.Exists(dir1) && !Directory.Exists(dir2)) Directory.Move(dir1, dir2);

            // Create a new directory if it doesn't already exist
            if (!Directory.Exists(dir2)) Directory.CreateDirectory(dir2);

            // Map the path to the JSON file
            string path = Path.Combine(dir2, $"Redirects_Backup_{DateTime.UtcNow:yyyyMMddHHmmss}.json");

            // Get the DTOs for all redirects in the database
            List<RedirectDto> dtos = database.Fetch<RedirectDto>("SELECT * FROM [SkybrudRedirects];");
            if (dtos.Count == 0) return new RedirectsBackupResult(path, dtos);

            // Save the JSON file to the disk
            JsonUtils.SaveJsonArray(path, JArray.FromObject(dtos));

            // Return the result
            return new RedirectsBackupResult(path, dtos);

        }

        internal static T GetService<T>() where T : notnull {
            return StaticServiceProvider.Instance.GetRequiredService<T>();
        }

    }

}