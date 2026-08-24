using System;
using Skybrud.Umbraco.Redirects.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;

namespace Skybrud.Umbraco.Redirects.PropertyEditors;

#pragma warning disable CS1591

/// <summary>
/// Property value converter for <see cref="InboundRedirectsPropertyEditor"/>.
/// </summary>
public class InboundRedirectsValueConverter : PropertyValueConverterBase {

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == InboundRedirectsPropertyEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source;
    }

    public override object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        return InboundRedirects.Instance;
    }

    public override Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return typeof(InboundRedirects);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.Element;
    }

}