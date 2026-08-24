using Skybrud.Umbraco.Redirects.Models.Settings;

namespace Skybrud.Umbraco.Redirects.Extensions;

/// <summary>
/// Static class with extension methods for <see cref="RedirectsDashboardSettings"/>.
/// </summary>
public static class RedirectsDashboardExtensions {

    /// <summary>
    /// Enables the dashboard.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDashboardSettings Enable(this RedirectsDashboardSettings settings) {
        settings.Enabled = true;
        return settings;
    }

    /// <summary>
    /// Disables the dashboard.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDashboardSettings Disable(this RedirectsDashboardSettings settings) {
        settings.Enabled = false;
        return settings;
    }

    /// <summary>
    /// Appends an allow rule for the user group with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the user group.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDashboardSettings Allow(this RedirectsDashboardSettings settings, string alias) {
        settings.UserGroups.Add($"+{alias}");
        return settings;
    }

    /// <summary>
    /// Appends an allow rule for all user groups.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDashboardSettings AllowAll(this RedirectsDashboardSettings settings) {
        settings.UserGroups.Add("+*");
        return settings;
    }

    /// <summary>
    /// Appends a disallow rule for the user group with the specified <paramref name="alias"/>.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <param name="alias">The alias of the user group.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDashboardSettings Disallow(this RedirectsDashboardSettings settings, string alias) {
        settings.UserGroups.Add($"-{alias}");
        return settings;
    }

    /// <summary>
    /// Appends a disallow rule for all user groups.
    /// </summary>
    /// <param name="settings">The settings instance.</param>
    /// <returns>The updated settings instance.</returns>
    public static RedirectsDashboardSettings DisallowAll(this RedirectsDashboardSettings settings) {
        settings.UserGroups.Add("-*");
        return settings;
    }

}