namespace Skybrud.Umbraco.Redirects.Config;

/// <summary>
/// Class representing the settings for the global redirects dashboard.
/// </summary>
public class RedirectsDashboardSettings {

    /// <summary>
    /// Gets whether the dashboard should be enabled. Default is <see langword="true"/>.
    /// </summary>
    public bool Enabled => true;

    /// <summary>
    /// Gets or sets the default limit of redirects to show in the dashboard. Default is <c>20</c>.
    /// </summary>
    public int Limit { get; set; } = 20;

}