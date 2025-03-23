using System.Collections.Generic;
using System.Threading.Tasks;
using Skybrud.Essentials.Security.Extensions;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Infrastructure.Manifest;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.Manifests;

public class RedirectsPackageManifestReader : IPackageManifestReader {

    public async Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync() {

        string cacheBuster = RedirectsPackage.InformationalVersion.ToMd5Hash();

        List<PackageManifest> temp = [
            new PackageManifest {
                Name = RedirectsPackage.Name,
                AllowTelemetry = true,
                Version = RedirectsPackage.InformationalVersion,
                Extensions = [
                    new {
                        name = "redirects.entrypoint",
                        alias = "Skybrud.Umbraco.Redirects.EntryPoint",
                        type = "backofficeEntryPoint",
                        js = "/App_Plugins/Skybrud.Umbraco.Redirects/EntryPoint.js?v=" + cacheBuster
                    }
                ],
                Importmap = new PackageManifestImportmap {
                    Imports = new Dictionary<string, string> {
                        {"@skybrud-redirects/auth", "/App_Plugins/Skybrud.Umbraco.Redirects/RedirectsAuth.js?v=" + cacheBuster},
                        {"@skybrud-redirects/package", "/App_Plugins/Skybrud.Umbraco.Redirects/RedirectsPackage.js?" + cacheBuster},
                        {"@skybrud-redirects/service", "/App_Plugins/Skybrud.Umbraco.Redirects/RedirectsService.js?v=" + cacheBuster},
                        {"@skybrud-redirects/modals/add", "/App_Plugins/Skybrud.Umbraco.Redirects/Modals/add-redirect.js?v=" + cacheBuster},
                        {"@skybrud-redirects/modals/edit", "/App_Plugins/Skybrud.Umbraco.Redirects/Modals/edit-redirect.js?v=" + cacheBuster},
                        {"@skybrud-redirects/elements/destination", "/App_Plugins/Skybrud.Umbraco.Redirects/Elements/Destination.js?v=" + cacheBuster}
                    }
                }
            }

        ];

        return await Task.FromResult(temp);

    }

}