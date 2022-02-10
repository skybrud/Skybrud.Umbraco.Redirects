using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

namespace Skybrud.Umbraco.Redirects.PropertyEditors {
    
    [DataEditor(EditorAlias, EditorType.PropertyValue, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Skybrud.dk", Icon = "icon-arrow-right")]
    public class OutboundRedirectEditor : DataEditor {

        public const string EditorAlias = "Skybrud.Umbraco.Redirects.OutboundRedirect";

        internal const string EditorName = "Skybrud.dk - Outbound redirect";

        internal const string EditorView = "/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Outbound.html";

        private readonly RedirectsBackOfficeHelper _backOfficeHelper;

        public OutboundRedirectEditor(IDataValueEditorFactory dataValueEditorFactory, RedirectsBackOfficeHelper backOfficeHelper) : base(dataValueEditorFactory) {
            _backOfficeHelper = backOfficeHelper;
        }

        public override IDataValueEditor GetValueEditor(object configuration) {
            
            IDataValueEditor editor = base.GetValueEditor(configuration);

            if (editor is DataValueEditor dve) {
                dve.View += $"?v={_backOfficeHelper.GetCacheBuster()}";
            }

            return editor;

        }

    }

}