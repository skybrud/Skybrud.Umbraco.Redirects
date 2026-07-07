using Skybrud.Umbraco.Redirects.Config.WorkspaceViews;

namespace Skybrud.Umbraco.Redirects.Extensions;

/// <summary>
/// Static class with extension methods for <see cref="RedirectsDocumentWorkspaceViewSettings"/>.
/// </summary>
public static class RedirectsDocumentWorkspaceViewExtensions {

    /// <summary>
    /// Enables the workspace view for <c>Document</c>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings Enable(this RedirectsDocumentWorkspaceViewSettings settings) {
        settings.Enabled = true;
        return settings;
    }

    /// <summary>
    /// Disables the workspace view for <c>Document</c>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings Disable(this RedirectsDocumentWorkspaceViewSettings settings) {
        settings.Enabled = false;
        return settings;
    }

    /// <summary>
    /// Appends an allow rule for the user group with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the user group.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings Allow(this RedirectsDocumentWorkspaceViewSettings settings, string alias) {
        settings.UserGroups.Add($"+{alias}");
        return settings;
    }

    /// <summary>
    /// Appends an allow rule for all user groups.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings AllowAll(this RedirectsDocumentWorkspaceViewSettings settings) {
        settings.UserGroups.Add("+*");
        return settings;
    }

    /// <summary>
    /// Appends a disallow rule for the user group with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the user group.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings Disallow(this RedirectsDocumentWorkspaceViewSettings settings, string alias) {
        settings.UserGroups.Add($"-{alias}");
        return settings;
    }

    /// <summary>
    /// Appends a disallow rule for all user groups.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings DisallowAll(this RedirectsDocumentWorkspaceViewSettings settings) {
        settings.UserGroups.Add("-*");
        return settings;
    }

    /// <summary>
    /// Appends a new include rule for the content type with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the content type.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings Include(this RedirectsDocumentWorkspaceViewSettings settings, string alias) {
        settings.ContentTypes.Add($"+{alias}");
        return settings;
    }

    /// <summary>
    /// Appends a new include rule for all content types.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings IncludeAll(this RedirectsDocumentWorkspaceViewSettings settings) {
        settings.ContentTypes.Add("+*");
        return settings;
    }

    /// <summary>
    /// Appends a new exclude rule for the content type with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the content type.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings Exclude(this RedirectsDocumentWorkspaceViewSettings settings, string alias) {
        settings.ContentTypes.Add($"-{alias}");
        return settings;
    }

    /// <summary>
    /// Appends a new exclude rule for all content types.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDocumentWorkspaceViewSettings ExcludeAll(this RedirectsDocumentWorkspaceViewSettings settings) {
        settings.ContentTypes.Add("-*");
        return settings;
    }

}