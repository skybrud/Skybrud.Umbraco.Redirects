import { UmbConditionBase } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_MEDIA_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/media";

export class RedirectsMediaCondition extends UmbConditionBase {
    constructor(host, args) {
        super(host, args);
        this.consumeContext(UMB_MEDIA_WORKSPACE_CONTEXT, (workspace) => {
            if (!workspace) return;
            this.observe(workspace.values, () => {
                const data = workspace.getData();
                if (!data || !data.mediaType) return;
                console.log(data);
                console.log(data.mediaType);
                this.permitted = data.mediaType.unique !== "f38bd2d7-65d0-48e6-95dc-87ce06ec2d3d";
                args.onChange();
            });
        });
    }
}

export default RedirectsMediaCondition;