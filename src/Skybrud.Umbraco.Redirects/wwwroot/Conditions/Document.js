import { UmbConditionBase } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

import { RedirectsPackage } from "@skybrud-redirects/package";
import { RedirectsService } from "@skybrud-redirects/service";

export class RedirectsDocumentCondition extends UmbConditionBase {

    #requireTemplate;
    #userGroups;
    #contentTypes;

    constructor(host, args) {
        super(host, args);
        this.#requireTemplate = RedirectsPackage.settings?.workspaceViews?.document?.requireTemplate !== false;
        this.#userGroups = RedirectsPackage.settings?.workspaceViews?.document?.userGroups ?? [];
        this.#contentTypes = RedirectsPackage.settings?.workspaceViews?.document?.contentTypes ?? [];
        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (workspace) => {
            if (!workspace) return;
            const data = workspace.getData();
            this.observe(workspace.structure.ownerContentType, (contentType) => {
                if (!contentType) return;
                RedirectsService.getCurrentUser().then((user) => {
                    this.permitted = this.#callback(data, contentType, workspace, user);
                    args.onChange();
                });
            });
        });
    }

    #callback(content, contentType, workspace, user) {

        const isNew = workspace.getIsNew();
        const hasTemplate = content?.template != null || content?.templateUnique != null || content?.templateKey != null;

        if (this.#requireTemplate && !hasTemplate) return false;

        if (!this.#byUser(user)) return false;

        if (this.#contentTypes.length > 0) {
            if (this.#contentTypes.includes("-" + contentType.alias)) return false;
            if (this.#contentTypes.includes("+" + contentType.alias)) return true;
            if (this.#contentTypes.includes("-*")) return false;
            if (this.#contentTypes.includes("+*")) return true;
            return !this.#contentTypes.some(x => x[0] === "+");
        }

        return true;

    }

    #byUser(user) {
        if (this.#userGroups.length === 0) return true;
        if (this.#userGroups.includes("+" + user.key)) return true;
        if (this.#userGroups.includes("-" + user.key)) return false;
        if (user.groups.some(group => this.#userGroups.includes("+" + group.alias))) return true;
        if (user.groups.some(group => this.#userGroups.includes("-" + group.alias))) return false;
        if (user.groups.some(group => this.#userGroups.includes("+*"))) return true;
        if (user.groups.some(group => this.#userGroups.includes("-*"))) return false;
        return !this.#userGroups.some(x => x[0] === "+");
    }

}

export default RedirectsDocumentCondition;