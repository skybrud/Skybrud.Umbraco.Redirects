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
        /// Gets or sets whether the user's start nodes should filter which redirects they have access to. Default is <see lanword="true"/>.
        /// </summary>
        public bool UserStartNodes { get; set; } = false;

    }

}