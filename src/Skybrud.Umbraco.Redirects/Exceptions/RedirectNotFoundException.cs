using System.Net;

namespace Skybrud.Umbraco.Redirects.Exceptions {

    /// <summary>
    /// Exception about a redirect that couldn't be found.
    /// </summary>
    public class RedirectNotFoundException : RedirectsException {

        /// <summary>
        /// Initializes a new exception with default message.
        /// </summary>
        public RedirectNotFoundException() : base(HttpStatusCode.NotFound, "A redirect with the specified ID does not exist.") { }
        
        /// <summary>
        /// Initializes a new exception with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The error message.</param>
        public RedirectNotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }

    }

}