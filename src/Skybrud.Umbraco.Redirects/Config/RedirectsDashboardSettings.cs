using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Config;

/// <summary>
/// Class representing the settings for the global redirects dashboard.
/// </summary>
public class RedirectsDashboardSettings {

    /// <summary>
    /// Gets whether the dashboard should be enabled. Default is <see langword="true"/>.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the weight of the dashboard. Default is <c>-10</c>.
    /// </summary>
    public int Weight { get; set; } = -10;

    /// <summary>
    /// Gets or sets the page size of the dashboard. Default is <c>20</c>.
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Gets or sets the debounce time in milliseconds for the dashboard. Default is <c>250</c>.
    /// </summary>
    public int Debounce { get; set; } = 250;

    /// <summary>
    /// Gets or sets a set of user groups for which the dashboard should be shown or hidden.
    /// </summary>
    public HashSet<string> UserGroups { get; set; } = [];

}