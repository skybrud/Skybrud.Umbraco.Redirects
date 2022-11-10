using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Config {

    /// <summary>
    /// Class with settings for the redirects content app.
    /// </summary>
    public class RedirectsContentAppSettings {

        /// <summary>
        /// Gets or sets whether the content app should be enabled. Default is <see langword="true"/>.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a list of content types and media types where the content app should or should not be shown.
        /// The format follows Umbraco's <c>show</c> option - eg. <c>+content/*</c> enables the content app for all
        /// content.
        ///
        /// If empty, the content app will be enabled for all content and media.
        /// </summary>
        public HashSet<string> Show { get; set; } = new();

    }

}