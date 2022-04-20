using Skybrud.Umbraco.Redirects.Helpers;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;

#pragma warning disable 1591

namespace Skybrud.Umbraco.Redirects.PropertyEditors {

    [DataEditor(EditorAlias, EditorType.PropertyValue, EditorName, EditorView, ValueType = ValueTypes.Json, Group = "Skybrud.dk", Icon = "icon-arrow-right color-skybrud")]
    public class InboundRedirectsEditor : DataEditor {

        internal const string EditorAlias = "Skybrud.Umbraco.Redirects";

        internal const string EditorName = "Skybrud.dk - Inbound redirects";

        internal const string EditorView = "/App_Plugins/Skybrud.Umbraco.Redirects/Views/Editors/Inbound.html";

        private readonly IIOHelper _ioHelper;
        private readonly RedirectsBackOfficeHelper _backOfficeHelper;

        public InboundRedirectsEditor(IIOHelper ioHelper, IDataValueEditorFactory dataValueEditorFactory, RedirectsBackOfficeHelper backOfficeHelper) : base(dataValueEditorFactory) {
            _ioHelper = ioHelper;
            _backOfficeHelper = backOfficeHelper;
        }
        
        protected override IConfigurationEditor CreateConfigurationEditor() => new InboundRedirectsConfigurationEditor(_ioHelper);

        public override IDataValueEditor GetValueEditor(object configuration) {
            
            IDataValueEditor editor = base.GetValueEditor(configuration);

            if (editor is DataValueEditor dve) {
                dve.View += $"?v={_backOfficeHelper.GetCacheBuster()}";
                dve.HideLabel = configuration is InboundRedirectsConfiguration {HideLabel: true};
            }

            return editor;

        }

    }

}