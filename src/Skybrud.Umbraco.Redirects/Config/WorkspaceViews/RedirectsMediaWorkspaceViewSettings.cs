namespace Skybrud.Umbraco.Redirects.Config.WorkspaceViews;

/// <summary>
/// Class representing the settings for the workspace view for <c>Media</c>.
/// </summary>
public class RedirectsMediaWorkspaceViewSettings {

    /// <summary>
    /// Gets or sets whether the workspace view for <c>Media</c> should be enabled. Default is <see langword="true"/>.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the weight of the workspace view for <c>Media</c>. Default is <c>-100</c>, which means it will be shown after the Umbraco <c>Info</c> workspace view.
    /// </summary>
    public int Weight { get; set; } = -100;

}