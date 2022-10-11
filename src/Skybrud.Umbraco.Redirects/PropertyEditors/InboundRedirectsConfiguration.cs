using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.PropertyEditors {

    public class InboundRedirectsConfiguration {

        [ConfigurationField("hideRootNodeOption", "Hide site option", "boolean", Description = "Specify whether the option to select a site should be hidden when creating or editing a redirect from the property editor.")]
        public bool HideRootNodeOption { get; set; }

        [ConfigurationField("hideTitle", "Hide title", "boolean", Description = "Specify whether the list title should be hidden.")]
        public bool HideTitle { get; set; }

        [ConfigurationField("hideLabel", "Hide label", "boolean", Description = "Specify whether the property editor label should be hidden.")]
        public bool HideLabel { get; set; }

    }

}