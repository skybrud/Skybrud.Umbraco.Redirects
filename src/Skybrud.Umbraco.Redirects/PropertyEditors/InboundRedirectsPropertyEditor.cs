using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class InboundRedirectsPropertyEditor : DataEditor {

    public const string EditorAlias = "Skybrud.Umbraco.Redirects";

    public const string EditorUiAlias = "Skybrud.Umbraco.Redirects.Ui";

    public InboundRedirectsPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

}