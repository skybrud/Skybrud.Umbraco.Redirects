using System.Collections.Generic;

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

    /// <summary>
    /// Gets or sets a set of user groups for which the workspace view for <c>Media</c> should be shown.
    /// </summary>
    public HashSet<string> UserGroups { get; set; } = []; // TODO: explain the format in the XML documentation

    /// <summary>
    /// Gets or sets a set of media types where the workspace view for <c>Media</c> should be shown.
    /// </summary>
    public HashSet<string> MediaTypes { get; set; } = []; // TODO: explain the format in the XML documentation

}