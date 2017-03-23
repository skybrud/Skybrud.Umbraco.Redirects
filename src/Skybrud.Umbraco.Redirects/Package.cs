using System;
using Semver;

namespace Skybrud.Umbraco.Redirects {

    public static class Package {

        public const string Alias = "Skybrud.Umbraco.Redirects";

        public const string Name = "Skybrud.Umbraco.Redirects";

        public static readonly Version Version = typeof(Package).Assembly.GetName().Version;

        public static readonly SemVersion SemVersion = new SemVersion(Version.Major, Version.Minor, Version.Build);

    }

}