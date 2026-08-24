using System.Collections.Generic;
using System.Threading.Tasks;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Manifests;

public class RedirectsPackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        const string alias = RedirectsPackage.Alias;
        string cacheBuster = RedirectsPackage.InformationalVersion.ToMd5Hash();

        List<PackageManifest> temp = [
            new PackageManifest {
                Id = RedirectsPackage.Alias,
                Name = RedirectsPackage.Name,
                AllowTelemetry = true,
                Version = RedirectsPackage.InformationalVersion,
                Extensions = [
                    new {
                        name = "Skybrud Redirects: Back-office Entry Point",
                        alias = "Skybrud.Umbraco.Redirects.EntryPoint",
                        type = "backofficeEntryPoint",
                        js = $"/App_Plugins/Skybrud.Umbraco.Redirects/EntryPoint.js?v={cacheBuster}"
                    }
                ],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string> {
                        {"@skybrud-redirects/auth", $"/App_Plugins/{alias}/RedirectsAuth.js?v={cacheBuster}"},
                        {"@skybrud-redirects/package", $"/App_Plugins/{alias}/RedirectsPackage.js?{cacheBuster}"},
                        {"@skybrud-redirects/service", $"/App_Plugins/{alias}/RedirectsService.js?v={cacheBuster}"},
                        {"@skybrud-redirects/modals/add", $"/App_Plugins/{alias}/Modals/add-redirect.js?v={cacheBuster}" },
                        {"@skybrud-redirects/modals/edit", $"/App_Plugins/{alias}/Modals/edit-redirect.js?v={cacheBuster}"},
                        {"@skybrud-redirects/elements/destination", $"/App_Plugins/{alias}/Elements/Destination.js?v={cacheBuster}"},
                        {"@skybrud-redirects/elements/from-now", $"/App_Plugins/{alias}/Elements/FromNow.js?v={cacheBuster}"},
                        {"@skybrud-redirects/elements/node", $"/App_Plugins/{alias}/Elements/Node.js?v={cacheBuster}"},
                        {"@skybrud-redirects/elements/base", $"/App_Plugins/{alias}/Elements/Base.js?v={cacheBuster}"},
                        {"@skybrud-redirects/events", $"/App_Plugins/{alias}/Events/Index.js?v={cacheBuster}"},
                        {"@skybrud-redirects/conditions/dashboard", $"/App_Plugins/{alias}/Conditions/Dashboard.js?v={cacheBuster}"},
                        {"@skybrud-redirects/conditions/document", $"/App_Plugins/{alias}/Conditions/Document.js?v={cacheBuster}"},
                        {"@skybrud-redirects/conditions/media", $"/App_Plugins/{alias}/Conditions/Media.js?v={cacheBuster}"}
                    }
                }
            }

        ];

        return await Task.FromResult(temp);

    }

}