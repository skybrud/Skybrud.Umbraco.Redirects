namespace Skybrud.Umbraco.Redirects.Config.WorkspaceViews;

/// <summary>
/// Class with settings for the redirects content app.
/// </summary>
public class RedirectsWorkspaceViewSettings {

    /// <summary>
    /// Gets or sets the settings for the workspace view for <c>Document</c>.
    /// </summary>
    public RedirectsDocumentWorkspaceViewSettings Document { get; set; } = new();

    /// <summary>
    /// Gets or sets the settings for the workspace view for <c>Media</c>.
    /// </summary>
    public RedirectsMediaWorkspaceViewSettings Media { get; set; } = new();

}