using System;
using System.Collections.Specialized;
using System.Web;
using Skybrud.Umbraco.Redirects.Models;

namespace Skybrud.Umbraco.Redirects.Events {
    
    public abstract class RedirectPreLookupEventArgs : EventArgs {

        /// <summary>
        /// Gets the inbound URI of the request.
        /// </summary>
        public abstract Uri Uri { get; }

        public abstract string RawUrl { get; }

        public abstract NameValueCollection QueryString { get; }

        /// <summary>
        /// Gets or sets the redirect. If a redirect is set from the pre lookup event, the redirects package will use
        /// this redirect, and skip any further lookups.
        /// </summary>
        public RedirectItem Redirect { get; set; }

        protected RedirectPreLookupEventArgs() { }

        protected RedirectPreLookupEventArgs(RedirectItem redirect) {
            Redirect = redirect;
        }

    }
    
    public class RedirectHttpContextPreLookupEventArgs : RedirectPreLookupEventArgs {
        
        public HttpContextBase HttpContext { get; protected set; }

        public override Uri Uri => HttpContext.Request.Url;

        public override string RawUrl => HttpContext.Request.RawUrl;

        public override NameValueCollection QueryString => HttpContext.Request.QueryString;

        public RedirectHttpContextPreLookupEventArgs(HttpContextBase context) {
            HttpContext = context;
        }

        public RedirectHttpContextPreLookupEventArgs(HttpContextBase context, RedirectItem redirect) {
            HttpContext = context;
            Redirect = redirect;
        }

    }

}