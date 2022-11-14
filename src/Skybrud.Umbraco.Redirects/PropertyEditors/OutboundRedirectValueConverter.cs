using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Umbraco.Redirects.Factories;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

namespace Skybrud.Umbraco.Redirects.PropertyEditors {

    internal class OutboundRedirectValueConverter : PropertyValueConverterBase {

        private readonly RedirectsModelsFactory _modelsFactory;

        #region Constructors

        public OutboundRedirectValueConverter(RedirectsModelsFactory modelsFactory) {
            _modelsFactory = modelsFactory;
        }

        #endregion

        #region Member methods

        public override bool IsConverter(IPublishedPropertyType propertyType) {
            return propertyType.EditorAlias == OutboundRedirectEditor.EditorAlias;
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview) {
            return source is string json && json.DetectIsJson() ? JsonUtils.ParseJsonObject(json) : null;
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return _modelsFactory.CreateOutboundRedirect(inter as JObject);
        }

        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return null;
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
            return PropertyCacheLevel.None;
        }

        public override System.Type GetPropertyValueType(IPublishedPropertyType propertyType) {
            return typeof(IOutboundRedirect);
        }

        #endregion

    }

}