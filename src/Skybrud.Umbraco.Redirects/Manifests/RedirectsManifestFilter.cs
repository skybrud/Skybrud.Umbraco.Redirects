using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Skybrud.Umbraco.Redirects.Manifests {

    /// <inheritdoc />
    public class RedirectsManifestFilter : IManifestFilter {

        /// <inheritdoc />
        public void Filter(List<PackageManifest> manifests) {

            // Initialize a new manifest filter for this package
            PackageManifest manifest = new() {
                AllowPackageTelemetry = true,
                PackageName = RedirectsPackage.Name,
                Version = RedirectsPackage.InformationalVersion.Split('+')[0],
                PackageId = RedirectsPackage.Alias,
                BundleOptions = BundleOptions.Independent,
                Scripts = new[] {
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Services/RedirectsService.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Dashboards/Default.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Dialogs/Redirect.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Editors/Culture.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Editors/Inbound.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Editors/Destination.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Editors/Outbound.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Editors/OriginalUrl.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Editors/RadioGroup.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Editors/Site.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Editors/Timestamp.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/ContentApp.js",
                    $"/App_Plugins/{RedirectsPackage.Alias}/Scripts/Controllers/Directives/Node.js"
                },
                Stylesheets = new[] {
                    $"/App_Plugins/{RedirectsPackage.Alias}/Styles/Styles.css"
                }
            };

            // Append the manifest
            manifests.Add(manifest);

        }

    }

}