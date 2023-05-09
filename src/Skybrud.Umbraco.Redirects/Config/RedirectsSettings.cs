namespace Skybrud.Umbraco.Redirects.Config {

    /// <summary>
    /// Class with settings for the redirects package.
    /// </summary>
    public class RedirectsSettings {

        /// <summary>
        /// Gets the settings for the redirects content app.
        /// </summary>
        public RedirectsContentAppSettings ContentApp { get; } = new();

        /// <summary>
        /// Gets the settings for the redirects dashboard.
        /// </summary>
        public RedirectsDashboardSettings Dashboard { get; } = new();

    }

}