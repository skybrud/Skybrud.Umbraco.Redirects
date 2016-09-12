using System;
using System.Net;

namespace Skybrud.Umbraco.Redirects.Exceptions {
    
    public class RedirectsException : Exception {

        public HttpStatusCode StatusCode { get; private set; }

        public RedirectsException(string message) : base(message) { }

        public RedirectsException(HttpStatusCode statusCode, string message) : base(message) {
            StatusCode = statusCode;
        }

        public RedirectsException(string message, HttpStatusCode statusCode) : base(message) {
            StatusCode = statusCode;
        }

    }

}