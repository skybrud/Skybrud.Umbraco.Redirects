using System.Collections.Generic;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Skybrud.Umbraco.Redirects.PropertyEditors;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.Factories.References {

    public class OutboundRedirectReferenceFactory : IDataValueReferenceFactory, IDataValueReference {

        public IDataValueReference GetDataValueReference() => this;

        public IEnumerable<UmbracoEntityReference> GetReferences(object? value) {

            List<UmbracoEntityReference> references = new List<UmbracoEntityReference>();
            if (value is not string json) return references;

            IRedirectDestination destination = OutboundRedirect.Deserialize(json).Destination;

            switch (destination.Type) {

                case RedirectDestinationType.Media:
                    references.Add(new UmbracoEntityReference(new GuidUdi("media", destination.Key)));
                    break;

                case RedirectDestinationType.Content:
                    references.Add(new UmbracoEntityReference(new GuidUdi("content", destination.Key)));
                    break;

            }

            return references;

        }

        public bool IsForEditor(IDataEditor? dataEditor) => dataEditor is not null && dataEditor.Alias.InvariantEquals(OutboundRedirectEditor.EditorAlias);

    }

}