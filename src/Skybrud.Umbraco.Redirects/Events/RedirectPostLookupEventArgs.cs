using Skybrud.Umbraco.Redirects.Models;
using System;
using System.Web;

namespace Skybrud.Umbraco.Redirects.Events {
    
    public class RedirectPostLookupEventArgs : EventArgs {
        
        public HttpContextBase HttpContext { get; }

        public RedirectItem Redirect { get; set; }

        public RedirectPostLookupEventArgs(HttpContext context, RedirectItem redirect) {
            HttpContext = new HttpContextWrapper(context);
            Redirect = redirect;
        }

    }

}