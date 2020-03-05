using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.Umbraco.Redirects.Models.Outbound {

    internal class OutboundRedirectPropertyValueConverter : PropertyValueConverterBase {

        public override bool IsConverter(IPublishedPropertyType propertyType) {
            return propertyType.EditorAlias == "Skybrud.Umbraco.Redirects.OutboundRedirect";
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object source, bool preview) {
            return OutboundRedirect.Deserialize(source as string);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return inter as OutboundRedirect;
        }

        public override object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return null;
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) => PropertyCacheLevel.None;

        public override System.Type GetPropertyValueType(IPublishedPropertyType propertyType) {
            return typeof(OutboundRedirect);
        }
    
    }

}