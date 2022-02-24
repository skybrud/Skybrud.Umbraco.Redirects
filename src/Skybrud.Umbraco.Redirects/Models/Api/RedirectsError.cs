using System.Net;
using Skybrud.Umbraco.Redirects.Exceptions;

namespace Skybrud.Umbraco.Redirects.Models.Api {
    
    /// <summary>
    /// Class representing a redirects error.
    /// </summary>
    public class RedirectsError {

        /// <summary>
        /// Gets the status code of the error.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the message of the error.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="error"/> message.
        /// </summary>
        /// <param name="error">The error message.</param>
        public RedirectsError(string error) {
            StatusCode = HttpStatusCode.InternalServerError;
            Error = error;
        } 
        
        /// <summary>
        /// Initializes a new instance based on the specified <paramref name="exception"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public RedirectsError(RedirectsException exception) {
            StatusCode = exception.StatusCode;
            Error = exception.Message;
        }

    }

}