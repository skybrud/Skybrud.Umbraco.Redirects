using System.Collections.Generic;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;

namespace Skybrud.Umbraco.Redirects.PropertyEditors {
    
    public class InboundRedirectsConfigurationEditor : ConfigurationEditor<InboundRedirectsConfiguration> {
        
        public InboundRedirectsConfigurationEditor(IIOHelper ioHelper) : base(ioHelper) { }

        public override IDictionary<string, object> DefaultConfiguration => new Dictionary<string, object> {
            {"hideTitle", true}
        };

    }

}