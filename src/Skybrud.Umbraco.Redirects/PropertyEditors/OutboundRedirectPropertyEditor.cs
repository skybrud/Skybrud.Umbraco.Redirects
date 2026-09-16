using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class OutboundRedirectPropertyEditor : DataEditor {

    public const string EditorName = "Limbo Separator";

    public const string EditorAlias = "Skybrud.Umbraco.Redirects.OutboundRedirect";

    public const string EditorUiAlias = "Skybrud.Umbraco.Redirects.OutboundRedirect.PropertyEditorUi";

    public const string EditorIcon = "icon-arrow-right";

    public const string EditorGroup = "Skybrud";

    public const string EditorValueType = ValueTypes.Json;

    public OutboundRedirectPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

}