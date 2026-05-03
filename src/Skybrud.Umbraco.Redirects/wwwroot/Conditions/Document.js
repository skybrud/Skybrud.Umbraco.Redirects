import { UmbConditionBase } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

export class RedirectsDocumentCondition extends UmbConditionBase {
    constructor(host, args) {
        super(host, args);
        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (workspace) => {
            if (!workspace) return;
            this.observe(workspace.values, () => {
                const data = workspace.getData();
                const isNew = workspace.getIsNew();
                const hasTemplate = data?.template != null || data?.templateUnique != null || data?.templateKey != null;
                this.permitted = !isNew && hasTemplate;
                args.onChange();
            });
        });
    }
}

export default RedirectsDocumentCondition;