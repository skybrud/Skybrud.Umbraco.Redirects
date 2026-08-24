using System.Diagnostics.CodeAnalysis;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Exceptions;

/// <summary>
/// Class representing an exception thrown when the user tries to create a redirect with an invalid original URL.
/// </summary>
public class RedirectsInvalidUrlException : RedirectsUserException {

    #region Properties

    /// <summary>
    /// Gets a reference to the redirect being created.
    /// </summary>
    public IRedirect Redirect { get; }

    /// <summary>
    /// Gets the original URL.
    /// </summary>
    public string OriginalUrl { get; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsInvalidUrlException"/> class with the redirect that contains the invalid URL.
    /// </summary>
    /// <param name="redirect">The redirect associated with the invalid URL.</param>
    [SetsRequiredMembers]
    public RedirectsInvalidUrlException(IRedirect redirect) : base("The original URL is not valid.") {
        UserMessageKey = "errorInvalidOriginalUrl";
        Redirect = redirect;
        OriginalUrl = redirect.Url;
    }

    #endregion

}