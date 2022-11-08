namespace Skybrud.Umbraco.Redirects.Config {

    /// <summary>
    /// Class with settings for the redirects package.
    /// </summary>
    public class RedirectsSettings {

        /// <summary>
        /// Gets settings for the redirects content app.
        /// </summary>
        public RedirectsContentAppSettings ContentApp { get; private set; } = new();

    }

}