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

        // Deny access when a template is required but the document doesn't have one
        if (this.#requireTemplate && !hasTemplate) return false;

        // Deny access when the current user isn't permitted
        if (!this.#byUser(user)) return false;

        // Check for restrictions based on the document's content type
        if (!this.#byContentType(contentType)) return false;

        return true;

    }

    #byContentType(contentType) {

        // Allow access when no content type restrictions have been configured
        if (this.#contentTypes.length === 0) return true;

        // Check for explicit access rules for the current content type
        if (this.#contentTypes.includes("+" + contentType.alias)) return true;
        if (this.#contentTypes.includes("-" + contentType.alias)) return false;

        // Deny access if an allow-list has been configured, otherwise allow access
        return !this.#contentTypes.some(x => x.startsWith("+"));

    }

    #byUser(user) {

        // Allow access when no user restrictions have been configured
        if (this.#userGroups.length === 0) return true;

        // Check for explicit access rules for the current user
        if (this.#userGroups.includes("+" + user.key)) return true;
        if (this.#userGroups.includes("-" + user.key)) return false;

        // Check for access rules matching any of the user's groups
        if (user.groups.some(group => this.#userGroups.includes("+" + group))) return true;
        if (user.groups.some(group => this.#userGroups.includes("-" + group))) return false;

        // Deny access if an allow-list has been configured, otherwise allow access
        return !this.#userGroups.some(x => x.startsWith("+"));

    }

}

export default RedirectsDocumentCondition;