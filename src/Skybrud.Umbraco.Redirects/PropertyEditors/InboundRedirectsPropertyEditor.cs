using Skybrud.Umbraco.Redirects.Constants;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.PropertyEditors;

[DataEditor(EditorAlias, ValueType = EditorValueType)]
public class InboundRedirectsPropertyEditor : DataEditor {

    public const string EditorName = "Skybrud Inbound Redirects";

    public const string EditorAlias = RedirectsPropertyEditorSchemaAliases.InboundRedirects;

    public const string EditorUiAlias = RedirectsPropertyEditorUiAliases.InboundRedirects;

    public const string EditorIcon = "icon-arrow-right";

    public const string EditorGroup = "Skybrud";

    public const string EditorValueType = ValueTypes.Json;

    public InboundRedirectsPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

}