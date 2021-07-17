using System;
using System.Web;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Events {
    
    public class RedirectPreLookupEventArgs : EventArgs {
        
        public HttpContextBase HttpContext { get; }

        public RedirectItem Redirect { get; set; }

        public RedirectPreLookupEventArgs(HttpContextBase context) {
            HttpContext = context;
        }

        public RedirectPreLookupEventArgs(HttpContextBase context, RedirectItem redirect) {
            HttpContext = context;
            Redirect = redirect;
        }

    }

}