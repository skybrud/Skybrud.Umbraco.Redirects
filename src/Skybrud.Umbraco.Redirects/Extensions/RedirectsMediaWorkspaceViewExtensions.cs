using Skybrud.Umbraco.Redirects.Models.Settings.WorkspaceViews;

namespace Skybrud.Umbraco.Redirects.Extensions;

/// <summary>
/// Static class with extension methods for <see cref="RedirectsMediaWorkspaceViewSettings"/>.
/// </summary>
public static class RedirectsMediaWorkspaceViewExtensions {

    /// <summary>
    /// Enables the workspace view for <c>Document</c>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsMediaWorkspaceViewSettings Enable(this RedirectsMediaWorkspaceViewSettings settings) {
        settings.Enabled = true;
        return settings;
    }

    /// <summary>
    /// Disables the workspace view for <c>Document</c>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsMediaWorkspaceViewSettings Disable(this RedirectsMediaWorkspaceViewSettings settings) {
        settings.Enabled = false;
        return settings;
    }

    /// <summary>
    /// Appends an allow rule for the user group with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the user group.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsMediaWorkspaceViewSettings AllowUserGroup(this RedirectsMediaWorkspaceViewSettings settings, string alias) {
        settings.UserGroups.Add($"+{alias}");
        return settings;
    }

    /// <summary>
    /// Appends a disallow rule for the user group with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the user group.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsMediaWorkspaceViewSettings DisallowUserGroup(this RedirectsMediaWorkspaceViewSettings settings, string alias) {
        settings.UserGroups.Add($"-{alias}");
        return settings;
    }

    /// <summary>
    /// Appends a new include rule for the media type with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the media type.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsMediaWorkspaceViewSettings IncludeMediaType(this RedirectsMediaWorkspaceViewSettings settings, string alias) {
        settings.MediaTypes.Add($"+{alias}");
        return settings;
    }

    /// <summary>
    /// Appends a new exclude rule for the media type with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the media type.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsMediaWorkspaceViewSettings ExcludeMediaType(this RedirectsMediaWorkspaceViewSettings settings, string alias) {
        settings.MediaTypes.Add($"-{alias}");
        return settings;
    }

}