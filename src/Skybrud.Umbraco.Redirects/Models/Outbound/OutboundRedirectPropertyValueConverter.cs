using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.Umbraco.Redirects.Models.Outbound {

    internal class OutboundRedirectPropertyValueConverter : PropertyValueConverterBase {

        public override bool IsConverter(PublishedPropertyType propertyType) {
            return propertyType.EditorAlias == "Skybrud.Umbraco.Redirects.OutboundRedirect";
        }

        public override object ConvertSourceToIntermediate(IPublishedElement owner, PublishedPropertyType propertyType, object source, bool preview) {
            return OutboundRedirect.Deserialize(source as string);
        }

        public override object ConvertIntermediateToObject(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return inter as OutboundRedirect;
        }

        public override object ConvertIntermediateToXPath(IPublishedElement owner, PublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview) {
            return null;
        }

        public override PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType) => PropertyCacheLevel.None;

        public override System.Type GetPropertyValueType(PublishedPropertyType propertyType) {
            return typeof(OutboundRedirect);
        }
    
    }

}