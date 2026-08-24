using System;
using System.Collections.Generic;

namespace Skybrud.Umbraco.Redirects.Exceptions;

/// <summary>
/// Class representing an exception that is intended to be shown to the user, typically with a user-friendly message.
/// </summary>
public class RedirectsUserException : RedirectsException {

    #region Properties

    /// <summary>
    /// The key for a user-friendly message that can be displayed to the user.
    /// </summary>
    public required string UserMessageKey { get; init; }

    /// <summary>
    /// Arguments for the user-friendly message, if any.
    /// </summary>
    public IReadOnlyList<object> UserMessageArgs { get; init; } = [];

    /// <summary>
    /// Gets or sets an additional data payload that will be returned with the error.
    /// </summary>
    public new object? Data { get; init; }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RedirectsUserException(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="data">An additional data payload that will be returned with the error.</param>
    public RedirectsUserException(string message, object? data) : base(message) {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RedirectsUserException(string message, Exception? innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="data">An additional data payload that will be returned with the error.</param>
    public RedirectsUserException(string message, object? data, Exception? innerException) : base(message, innerException) {
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a developer message and a user message key.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    public RedirectsUserException(string developerMessage, string userMessageKey) : base(developerMessage) {
        UserMessageKey = userMessageKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a developer message and a user message key.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="data">An additional data payload that will be returned with the error.</param>
    public RedirectsUserException(string developerMessage, string userMessageKey, object? data) : base(developerMessage) {
        UserMessageKey = userMessageKey;
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a developer message, a user message key, and an inner exception.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RedirectsUserException(string developerMessage, string userMessageKey, Exception? innerException) : base(developerMessage, innerException) {
        UserMessageKey = userMessageKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a developer message, a user message key, and an inner exception.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    /// <param name="data">An additional data payload that will be returned with the error.</param>
    public RedirectsUserException(string developerMessage, string userMessageKey, object? data, Exception? innerException) : base(developerMessage, innerException) {
        UserMessageKey = userMessageKey;
        Data = data;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a developer message, a user message key, and user message arguments.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="userMessageArgs">Arguments for the user-friendly message.</param>
    public RedirectsUserException(string developerMessage, string userMessageKey, object[]? userMessageArgs) : base(developerMessage) {
        UserMessageKey = userMessageKey;
        UserMessageArgs = userMessageArgs ?? [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a developer message, a user message key, and user message arguments.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="userMessageArgs">Arguments for the user-friendly message.</param>
    /// <param name="data">An additional data payload that will be returned with the error.</param>
    public RedirectsUserException(string developerMessage, string userMessageKey, object[]? userMessageArgs, object? data) : base(developerMessage) {
        UserMessageKey = userMessageKey;
        Data = data;
        UserMessageArgs = userMessageArgs ?? [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a developer message, a user message key, user message arguments, and an inner exception.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="userMessageArgs">Arguments for the user-friendly message.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RedirectsUserException(string developerMessage, string userMessageKey, object[]? userMessageArgs, Exception? innerException) : base(developerMessage, innerException) {
        UserMessageKey = userMessageKey;
        UserMessageArgs = userMessageArgs ?? [];
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedirectsUserException"/> class with a developer message, a user message key, user message arguments, and an inner exception.
    /// </summary>
    /// <param name="developerMessage">The message intended for developers.</param>
    /// <param name="userMessageKey">The key for the user-friendly message.</param>
    /// <param name="userMessageArgs">Arguments for the user-friendly message.</param>
    /// <param name="data">An additional data payload that will be returned with the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public RedirectsUserException(string developerMessage, string userMessageKey, object[]? userMessageArgs, object? data, Exception? innerException) : base(developerMessage, innerException) {
        UserMessageKey = userMessageKey;
        Data = data;
        UserMessageArgs = userMessageArgs ?? [];
    }

    #endregion

}