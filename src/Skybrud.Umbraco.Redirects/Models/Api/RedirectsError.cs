using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Skybrud.Umbraco.Redirects.Exceptions;

namespace Skybrud.Umbraco.Redirects.Models.Api {
    
    public class RedirectsError {

        public HttpStatusCode StatusCode { get; }

        public string Error { get; }

        public RedirectsError(string error) {
            StatusCode = HttpStatusCode.InternalServerError;
            Error = error;
        } 
        
        public RedirectsError(RedirectsException exception) {
            StatusCode = exception.StatusCode;
            Error = exception.Message;
        }

    }

}