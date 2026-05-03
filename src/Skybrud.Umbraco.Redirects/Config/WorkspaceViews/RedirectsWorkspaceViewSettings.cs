namespace Skybrud.Umbraco.Redirects.Config.WorkspaceViews;

/// <summary>
/// Class with settings for the redirects content app.
/// </summary>
public class RedirectsWorkspaceViewSettings {

    ///// <summary>
    ///// Gets or sets whether the content app should be enabled. Default is <see langword="true"/>.
    ///// </summary>
    //public bool Enabled { get; set; } = true;

    ///// <summary>
    ///// Gets or sets a list of content types and media types where the content app should or should not be shown.
    ///// The format follows Umbraco's <c>show</c> option - e.g. <c>+content/*</c> enables the content app for all
    ///// content.
    /////
    ///// If empty, the content app will be enabled for all content and media.
    ///// </summary>
    //public HashSet<string> Show { get; set; } = [];

    /// <summary>
    /// Gets or sets the settings for the workspace view for <c>Document</c>.
    /// </summary>
    public RedirectsDocumentWorkspaceViewSettings Document { get; set; } = new();

    /// <summary>
    /// Gets or sets the settings for the workspace view for <c>Media</c>.
    /// </summary>
    public RedirectsMediaWorkspaceViewSettings Media { get; set; } = new();

}