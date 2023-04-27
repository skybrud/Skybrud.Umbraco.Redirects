using System;
using System.Globalization;
using System.Net;
using Umbraco.Cms.Core.Services;

namespace Skybrud.Umbraco.Redirects.Exceptions {

    /// <summary>
    /// Exception representing a generic error thrown by the logic within this package.
    /// </summary>
    public abstract class RedirectsLocalizedException : RedirectsException {

        /// <summary>
        /// Initializes a new exception with the specified <paramref name="message"/>. <see cref="RedirectsException.StatusCode"/> will be <see cref="HttpStatusCode.InternalServerError"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        protected RedirectsLocalizedException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new exception with the specified <paramref name="statusCode"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The error message.</param>
        protected RedirectsLocalizedException(HttpStatusCode statusCode, string message) : base(statusCode, message) { }

        /// <summary>
        /// Initializes a new exception with the specified <paramref name="message"/> and <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The status code.</param>
        protected RedirectsLocalizedException(string message, HttpStatusCode statusCode) : base(message, statusCode) { }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="message"/> and <paramref name="innerException"/>.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        /// <param name="innerException">An inner exception.</param>
        protected RedirectsLocalizedException(string message, Exception innerException) : base(message, innerException) { }

        internal abstract string GetLocalizedMessage(ILocalizedTextService textService, CultureInfo? culture);

    }

}