using System;
using System.Web;

namespace Skybrud.Umbraco.Redirects.Events {
    
    public class RedirectPreLookupEventArgs : EventArgs {
        
        public HttpContextBase HttpContext { get; }

        public string RawUrl { get; set; }

        public RedirectPreLookupEventArgs(HttpContext context, string rawUrl) {
            HttpContext = new HttpContextWrapper(context);
            RawUrl = rawUrl;
        }

    }

}