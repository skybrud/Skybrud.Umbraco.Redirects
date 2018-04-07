using System;
using System.Net;

namespace Skybrud.Umbraco.Redirects.Exceptions {
    
    /// <summary>
    /// Exception representing a generic error thrown by the logic within this package.
    /// </summary>
    public class RedirectsException : Exception {

        /// <summary>
        /// The status code of the error.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Initializes a new exception with the specified <paramref name="message"/>. <see cref="StatusCode"/> will be <see cref="HttpStatusCode.InternalServerError"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RedirectsException(string message) : base(message) {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        /// <summary>
        /// Initializes a new exception with the specified <paramref name="statusCode"/> and <paramref name="message"/>.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        /// <param name="message">The error message.</param>
        public RedirectsException(HttpStatusCode statusCode, string message) : base(message) {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Initializes a new exception with the specified <paramref name="message"/> and <paramref name="statusCode"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The status code.</param>
        public RedirectsException(string message, HttpStatusCode statusCode) : base(message) {
            StatusCode = statusCode;
        }

    }

}