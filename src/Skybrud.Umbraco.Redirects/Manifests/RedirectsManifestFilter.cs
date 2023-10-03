using System.Collections.Generic;
using System.Reflection;
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
                Version = RedirectsPackage.InformationalVersion,
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

            // The "PackageId" property isn't available prior to Umbraco 12, and since the package is build against
            // Umbraco 10, we need to use reflection for setting the property value for Umbraco 12+. Ideally this
            // shouldn't fail, but we might at least add a try/catch to be sure
            try {
                PropertyInfo? property = manifest.GetType().GetProperty("PackageId");
                property?.SetValue(manifest, RedirectsPackage.Alias);
            } catch {
                // We don't really care about the exception
            }

            // Append the manifest
            manifests.Add(manifest);

        }

    }

}