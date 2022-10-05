using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.PropertyEditors {

    public class InboundRedirectsConfigurationEditor : ConfigurationEditor<InboundRedirectsConfiguration> {

        public InboundRedirectsConfigurationEditor(IIOHelper ioHelper, IEditorConfigurationParser editorConfigurationParser) : base(ioHelper, editorConfigurationParser) { }

        public override IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object> {
            {"hideTitle", true}
        };

    }

}