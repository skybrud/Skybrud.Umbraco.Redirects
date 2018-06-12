using System;
using Semver;

namespace Skybrud.Umbraco.Redirects {

    /// <summary>
    /// Static class with various information and constants about the package.
    /// </summary>
    public static class Package {

        /// <summary>
        /// Gets the alias of the package.
        /// </summary>
        public const string Alias = "Skybrud.Umbraco.Redirects";

        /// <summary>
        /// Gets the friendly name of the package.
        /// </summary>
        public const string Name = "Skybrud.Umbraco.Redirects";

        /// <summary>
        /// Gets the version of the package.
        /// </summary>
        public static readonly Version Version = typeof(Package).Assembly.GetName().Version;

        /// <summary>
        /// Gets the semantic version of the package.
        /// </summary>
        public static readonly SemVersion SemVersion = new SemVersion(Version.Major, Version.Minor, Version.Build);

    }

}