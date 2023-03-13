using System;
using System.Diagnostics;
using Umbraco.Cms.Core.Semver;

namespace Skybrud.Umbraco.Redirects {

    /// <summary>
    /// Static class with various information and constants about the package.
    /// </summary>
    public static class RedirectsPackage {

        /// <summary>
        /// Gets the alias of the package.
        /// </summary>
        public const string Alias = "Skybrud.Umbraco.Redirects";

        /// <summary>
        /// Gets the friendly name of the package.
        /// </summary>
        public const string Name = "Skybrud Redirects";

        /// <summary>
        /// Gets the version of the package.
        /// </summary>
        public static readonly Version Version = typeof(RedirectsPackage).Assembly.GetName().Version!;

        /// <summary>
        /// Gets the informational version of the package.
        /// </summary>
        public static readonly string InformationalVersion = FileVersionInfo.GetVersionInfo(typeof(RedirectsPackage).Assembly.Location).ProductVersion!;

        /// <summary>
        /// Gets the semantic version of the package.
        /// </summary>
        public static readonly SemVersion SemVersion = InformationalVersion;

        /// <summary>
        /// Gets the URL of the GitHub repository for this package.
        /// </summary>
        public const string GitHubUrl = "https://github.com/skybrud/Skybrud.Umbraco.Redirects";

        /// <summary>
        /// Gets the URL of the issue tracker for this package.
        /// </summary>
        public const string IssuesUrl = "https://github.com/skybrud/Skybrud.Umbraco.Redirects/issues";

        /// <summary>
        /// Gets the URL of the documentation for this package.
        /// </summary>
        public const string DocumentationUrl = "https://packages.skybrud.dk/skybrud.umbraco.redirects/";

    }

}