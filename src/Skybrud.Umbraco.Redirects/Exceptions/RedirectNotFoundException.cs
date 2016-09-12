using System.Net;

namespace Skybrud.Umbraco.Redirects.Exceptions {

    public class RedirectNotFoundException : RedirectsException {

        public RedirectNotFoundException() : base(HttpStatusCode.NotFound, "A redirect with the specified ID does not exist.") { }

        public RedirectNotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }

    }

}