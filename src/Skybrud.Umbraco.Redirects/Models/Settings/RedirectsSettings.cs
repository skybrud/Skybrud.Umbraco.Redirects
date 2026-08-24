using Skybrud.Umbraco.Redirects.Models.Settings.WorkspaceViews;

namespace Skybrud.Umbraco.Redirects.Models.Settings;

/// <summary>
/// Class with settings for the redirects package.
/// </summary>
public class RedirectsSettings {

    /// <summary>
    /// Gets or sets the frontend URL. If specified, this will be used for determining an absolute
    /// inbound URL for redirects shown in the dashboard in the Umbraco back-office.
    /// </summary>
    public string? FrontendUrl { get; set; }

    /// <summary>
    /// Gets the settings for the redirects dashboard.
    /// </summary>
    public RedirectsDashboardSettings Dashboard { get; } = new();

    /// <summary>
    /// Gets the settings for the redirects workspace views.
    /// </summary>
    public RedirectsWorkspaceViewSettings WorkspaceViews { get; } = new();

}