using System;
using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Models.Api;

/// <summary>
/// Model class representing a user in the Umbraco backoffice.
/// </summary>
public class ApiUserItem {

    /// <summary>
    /// Gets or sets the numeric ID of the user.
    /// </summary>
    public required int Id { get; init; }

    /// <summary>
    /// Gets or sets the GUID key of the user.
    /// </summary>
    public required Guid Key { get; init; }

    /// <summary>
    /// Gets or sets a list with the aliases of the user's groups.
    /// </summary>
    public required IReadOnlyList<string> Groups { get; init; }

}