using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Config.WorkspaceViews;

/// <summary>
/// Class representing the settings for the workspace view for <c>Document</c>.
/// </summary>
public class RedirectsDocumentWorkspaceViewSettings {

    /// <summary>
    /// Gets or sets whether the workspace view for <c>Document</c> should be enabled. Default is <see langword="true"/>.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the weight of the workspace view for <c>Document</c>. Default is <c>-100</c>, which means it will be shown after the Umbraco <c>Info</c> workspace view.
    /// </summary>
    public int Weight { get; set; } = -100;

    /// <summary>
    /// Gets or sets whether the workspace view for <c>Document</c> should require a template. If <see langword="true"/>, the workspace view will only be shown if the document has a template assigned. Default is <see langword="true"/>.
    /// </summary>
    public bool RequireTemplate { get; set; } = true;

    /// <summary>
    /// Gets or sets a set of content types where the workspace view for <c>Document</c> should be shown.
    /// </summary>
    public HashSet<string> Show { get; set; } = []; // TODO: explain the format in the XML documentation

}