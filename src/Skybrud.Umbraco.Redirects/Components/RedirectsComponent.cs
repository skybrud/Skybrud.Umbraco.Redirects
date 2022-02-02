using System.Collections.Generic;
using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Core.Composing;
using Umbraco.Web.JavaScript;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Components {
    
    public class RedirectsComponent : IComponent {
        
        private readonly RedirectsBackOfficeHelper _backoffice;

        public RedirectsComponent(RedirectsBackOfficeHelper backoffice) {
            _backoffice = backoffice;
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

            // Append the "redirects" dictionary to "skybrud"
            skybrud.Add("redirects", _backoffice.GetServerVariables());

        }

    }

}