using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Umbraco.Redirects.Models;
using Skybrud.Umbraco.Redirects.PropertyEditors;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.Editors;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable 1591

// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault

namespace Skybrud.Umbraco.Redirects.Factories.References {

    public class OutboundRedirectReferenceFactory : IDataValueReferenceFactory, IDataValueReference {

        private readonly RedirectsModelsFactory _modelsFactory;

        #region Constructors

        public OutboundRedirectReferenceFactory(RedirectsModelsFactory modelsFactory)  {
            _modelsFactory = modelsFactory;
        }

        #endregion

        #region Member methods

        public IDataValueReference GetDataValueReference() => this;

        public IEnumerable<UmbracoEntityReference> GetReferences(object? value) {

            List<UmbracoEntityReference> references = new();
            if (value is not string json) return references;

            // Parse the raw JSON value
            if (!JsonUtils.TryParseJsonObject(json, out JObject? result)) return references;

            // Parse the JSON object into a "IRedirectDestination" via the models factory
            IRedirectDestination? destination = _modelsFactory.CreateOutboundRedirect(result)?.Destination;

            // Handle "Media" and "Content" (but not "Url")
            switch (destination?.Type) {

                case RedirectDestinationType.Media:
                    references.Add(new UmbracoEntityReference(new GuidUdi(Constants.UdiEntityType.Media, destination.Key)));
                    break;

                case RedirectDestinationType.Content:
                    references.Add(new UmbracoEntityReference(new GuidUdi(Constants.UdiEntityType.Document, destination.Key)));
                    break;

            }

            return references;

        }

        public bool IsForEditor(IDataEditor? dataEditor) => dataEditor is not null && dataEditor.Alias.InvariantEquals(OutboundRedirectEditor.EditorAlias);

        #endregion

    }

}