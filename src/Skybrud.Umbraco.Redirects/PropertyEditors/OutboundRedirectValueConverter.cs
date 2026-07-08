using Newtonsoft.Json.Linq;
using Skybrud.Essentials.Json.Newtonsoft;
using Skybrud.Umbraco.Redirects.Factories;
using Skybrud.Umbraco.Redirects.Models.Outbound;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Extensions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.PropertyEditors;


/// <summary>
/// Property value converter for <see cref="OutboundRedirectPropertyEditor"/>.
/// </summary>
public class OutboundRedirectValueConverter : PropertyValueConverterBase {

    private readonly RedirectsModelFactory _modelsFactory;

    #region Constructors

    public OutboundRedirectValueConverter(RedirectsModelFactory modelsFactory) {
        _modelsFactory = modelsFactory;
    }

    #endregion

    #region Member methods

    public override bool IsConverter(IPublishedPropertyType propertyType) {
        return propertyType.EditorAlias == OutboundRedirectPropertyEditor.EditorAlias;
    }

    public override object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview) {
        return source is string json && json.DetectIsJson() ? JsonUtils.ParseJsonObject(json) : null;
    }

    public override object? ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview) {
        return _modelsFactory.ParseOutboundRedirect(inter as JObject);
    }

    public override PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType) {
        return PropertyCacheLevel.None;
    }

    public override System.Type GetPropertyValueType(IPublishedPropertyType propertyType) {
        return typeof(IOutboundRedirect);
    }

    #endregion

}