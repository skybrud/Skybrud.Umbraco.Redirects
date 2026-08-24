using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Skybrud.Umbraco.Redirects.PropertyEditors;

[DataEditor(EditorAlias, ValueType = ValueTypes.Json)]
public class InboundRedirectsPropertyEditor : DataEditor {

    internal const string EditorAlias = "Skybrud.Umbraco.Redirects";

    internal const string EditorUiAlias = "Skybrud.Umbraco.Redirects.Ui";

    public InboundRedirectsPropertyEditor(IDataValueEditorFactory dataValueEditorFactory) : base(dataValueEditorFactory) { }

}