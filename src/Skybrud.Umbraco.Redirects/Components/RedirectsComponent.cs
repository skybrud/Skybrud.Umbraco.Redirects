using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Skybrud.Umbraco.Redirects.Controllers.Api;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;

namespace Skybrud.Umbraco.Redirects.Components {
    
    public class RedirectsComponent : IComponent {
        
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;

        public RedirectsComponent(IUmbracoContextAccessor umbracoContextAccessor) {
            _umbracoContextAccessor = umbracoContextAccessor;
        }
        
        public void Initialize() {
            ServerVariablesParser.Parsing += ServerVariablesParserOnParsing;
        }

        public void Terminate() {
            ServerVariablesParser.Parsing -= ServerVariablesParserOnParsing;
        }

        private void ServerVariablesParserOnParsing(object sender, Dictionary<string, object> e) {

            // Get or create the "skybrud" dictionary
            if (!(e.TryGetValue("skybrud", out object value) && value is Dictionary<string, object> skybrud))  {
                e["skybrud"] = skybrud = new Dictionary<string, object>();
            }

            // Determine the API base URL
            string baseUrl = null;
            if (TryCreateUrlHelper(out UrlHelper url)) {
                baseUrl = url.GetUmbracoApiService<RedirectsController>("Dummy").TrimEnd("Dummy");
            }

            // Append the "redirects" dictionary to "skybrud"
            skybrud.Add("redirects", new Dictionary<string, object> {
                { "baseUrl", baseUrl }
            });

        }

        private bool TryCreateUrlHelper(out UrlHelper helper) {

            // Get the current HTTP context via the Umbraco context accessor
            HttpContextBase http = _umbracoContextAccessor.UmbracoContext.HttpContext;
            if (http == null) {
                helper = null;
                return false;
            }

            // Initialize a new URL helper
            helper = new UrlHelper(http.Request.RequestContext);
            return true;

        }

    }

}