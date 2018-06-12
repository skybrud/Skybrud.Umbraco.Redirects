using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors;

namespace Skybrud.Umbraco.Redirects.Models.Outbound {

    internal class OutboundRedirectPropertyValueConverter : PropertyValueConverterBase, IPropertyValueConverterMeta {

        public override bool IsConverter(PublishedPropertyType propertyType) {
            return propertyType.PropertyEditorAlias == "Skybrud.Umbraco.Redirects.OutboundRedirect";
        }

        public override object ConvertDataToSource(PublishedPropertyType propertyType, object data, bool preview) {
            return OutboundRedirect.Deserialize(data as string);
        }

        public override object ConvertSourceToObject(PublishedPropertyType propertyType, object source, bool preview) {
            return source as OutboundRedirect;
        }

        public override object ConvertSourceToXPath(PublishedPropertyType propertyType, object source, bool preview) {
            return null;
        }

        public PropertyCacheLevel GetPropertyCacheLevel(PublishedPropertyType propertyType, PropertyCacheValue cacheValue) {
            switch (cacheValue) {
                case PropertyCacheValue.Object: return PropertyCacheLevel.ContentCache;
                case PropertyCacheValue.Source: return PropertyCacheLevel.Content;
                case PropertyCacheValue.XPath: return PropertyCacheLevel.Content;
                default: return PropertyCacheLevel.None;
            }
        }

        public System.Type GetPropertyValueType(PublishedPropertyType propertyType) {
            return typeof(OutboundRedirect);
        }
    
    }

}