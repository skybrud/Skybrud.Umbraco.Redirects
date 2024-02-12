using System.Collections.Generic;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Skybrud.Umbraco.Redirects.PropertyEditors;
using Umbraco.Core;
using Umbraco.Core.Models.Editors;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.Umbraco.Redirects.Factories.References {
    
    internal class OutboundRedirectReferenceFactory : IDataValueReferenceFactory, IDataValueReference {

        public IDataValueReference GetDataValueReference() => this;

        public IEnumerable<UmbracoEntityReference> GetReferences(object value) {
            
            List<UmbracoEntityReference> references = new List<UmbracoEntityReference>();
            if (value is not string json) return references;

            RedirectDestination destination = OutboundRedirect.Deserialize(json)?.Destination;
            if (destination == null) return references;

            switch (destination.Type) {
                
                case RedirectDestinationType.Media:
                    references.Add(new UmbracoEntityReference(new GuidUdi("media", destination.Key)));
                    break;
                
                case RedirectDestinationType.Content:
                    references.Add(new UmbracoEntityReference(new GuidUdi("document", destination.Key)));
                    break;

            }
            
            return references;

        }

        public bool IsForEditor(IDataEditor dataEditor) => dataEditor.Alias.InvariantEquals(OutboundRedirectEditor.EditorAlias);

    }

}