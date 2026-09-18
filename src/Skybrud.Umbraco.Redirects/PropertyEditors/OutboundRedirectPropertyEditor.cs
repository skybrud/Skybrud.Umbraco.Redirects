using Skybrud.Umbraco.Redirects.Constants;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class OutboundRedirectPropertyEditor : DataEditor {

    public const string EditorName = "Skybrud Outbound Redirect";

    public const string EditorAlias = RedirectsPropertyEditorSchemaAliases.OutboundRedirect;

    public const string EditorUiAlias = RedirectsPropertyEditorUiAliases.OutboundRedirect;

    public const string EditorIcon = "icon-arrow-right";

    public const string EditorGroup = "Skybrud";

    public const string EditorValueType = ValueTypes.Json;

    public OutboundRedirectPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

}