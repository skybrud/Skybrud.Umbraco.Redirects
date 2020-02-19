using System;
using System.Xml.Linq;
using System.Xml.XPath;
using Umbraco.Core.Composing;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.PackageActions;

namespace Skybrud.Umbraco.Redirects.PackageActions {

    public class AddRedirectsModuleAction : IPackageAction {

        protected const string ModuleAlias = "RedirectsModule";

        public bool Execute(string packageName, XElement xmlData) {
            
            string path = IOHelper.MapPath("~/Web.config");

            try {

                // Load the XML document
                XDocument document = XDocument.Load(path, LoadOptions.PreserveWhitespace);

                // Locate the <module> element
                XElement modules = document.Root.XPathSelectElement("//configuration/system.webServer/modules");
                if (modules == null) throw new Exception("<modules> element not found.");

                // Stop execution if the module has already been added
                XElement redirectsModule = modules.XPathSelectElement($"add[@name='{ModuleAlias}']");
                if (redirectsModule != null) return true;

                // Attempt to find "UmbracoModule"
                XElement xmlUmbracoModule = modules.XPathSelectElement("add[@name='UmbracoModule']");
                if (xmlUmbracoModule == null) throw new Exception("'UmbracoModule' HTTP module not found.");

                // Add the redirects module right after "UmbracoModule"
                xmlUmbracoModule.AddAfterSelf(new XElement(
                    "add",
                    new XAttribute("name", ModuleAlias),
                    new XAttribute("type", "Skybrud.Umbraco.Redirects.Routing.RedirectsModule, Skybrud.Umbraco.Redirects")
                ));

                // Then add the <remove> element right after the "UmbracoModule" (so it's before the <add> element)
                xmlUmbracoModule.AddAfterSelf(new XElement(
                    "remove",
                    new XAttribute("name", ModuleAlias)
                ));

                // Save the document
                document.Save(path);

                return true;

            } catch (Exception ex) {

                Current.Logger.Error<AddRedirectsModuleAction>("Failed adding redirects HTTP module", ex);

                return false;

            }

        }

        public string Alias() {
            return "AddSkybrudRedirectsModule";
        }

        public bool Undo(string packageName, XElement xmlData) {

            string path = IOHelper.MapPath("~/Web.config");

            try {

                // Load the XML document
                XDocument document = XDocument.Load(path, LoadOptions.PreserveWhitespace);

                // Locate the <module> element
                XElement modules = document.Root.XPathSelectElement("//configuration/system.webServer/modules");
                if (modules == null) throw new Exception("<modules> element not found.");

                // Track whetehr we're making any changes to the XML document
                bool updated = false;

                // Remove the <add> element
                XElement add = modules.XPathSelectElement($"add[@name='{ModuleAlias}']");
                if (add != null) {
                    add.Remove();
                    updated = true;
                }

                // Remove the <remove> element
                XElement remove = modules.XPathSelectElement($"remove[@name='{ModuleAlias}']");
                if (remove != null) {
                    remove.Remove();
                    updated = true;
                }

                // Save the document if we made any changes
                if (updated) document.Save(path);

                return true;

            } catch (Exception ex) {

                Current.Logger.Error<AddRedirectsModuleAction>("Failed removing redirects HTTP module", ex);

                return false;

            }

        }

    }

}