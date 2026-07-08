import { UmbConditionBase } from "@umbraco-cms/backoffice/extension-registry";
import { UMB_MEDIA_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/media";

import { RedirectsPackage } from "@skybrud-redirects/package";
import { RedirectsService } from "@skybrud-redirects/service";

const FOLDER_MEDIA_TYPE_KEY = "f38bd2d7-65d0-48e6-95dc-87ce06ec2d3d";

export class RedirectsMediaCondition extends UmbConditionBase {

    #userGroups;
    #mediaTypes;

    constructor(host, args) {
        super(host, args);
        this.#userGroups = RedirectsPackage.settings?.workspaceViews?.media?.userGroups ?? [];
        this.#mediaTypes = RedirectsPackage.settings?.workspaceViews?.media?.contentTypes ?? [];
        this.consumeContext(UMB_MEDIA_WORKSPACE_CONTEXT, (workspace) => {
            if (!workspace) return;
            const media = workspace.getData();
            this.observe(workspace.structure.ownerContentType, (mediaType) => {
                if (!mediaType) return;
                RedirectsService.getCurrentUser().then((user) => {
                    this.permitted = this.#callback(media, mediaType, workspace, user);
                    args.onChange();
                });
            });
        });
    }

    #callback(media, mediaType, workspace, user) {

        // Folders cant have URLs, so we disable the workspace view for folders
        if (mediaType.unique === FOLDER_MEDIA_TYPE_KEY) return false;

        // TODO: should we look up whether the media has any URLs?

        if (!this.#byUser(user)) return false;

        if (this.#mediaTypes.length > 0) {
            if (this.#mediaTypes.includes("-" + mediaType.alias)) return false;
            if (this.#mediaTypes.includes("+" + mediaType.alias)) return true;
            if (this.#mediaTypes.includes("-*")) return false;
            if (this.#mediaTypes.includes("+*")) return true;
            return !this.#mediaTypes.some(x => x[0] === "+");
            return false;
        }

        return true;

    }

    #byUser(user) {
        if (this.#userGroups.length === 0) return true;
        if (this.#userGroups.includes("+" + user.key)) return true;
        if (this.#userGroups.includes("-" + user.key)) return false;
        if (user.groups.some(group => this.#userGroups.includes("+" + group))) return true;
        if (user.groups.some(group => this.#userGroups.includes("-" + group))) return false;
        if (user.groups.some(group => this.#userGroups.includes("+*"))) return true;
        if (user.groups.some(group => this.#userGroups.includes("-*"))) return false;
        return !this.#userGroups.some(x => x[0] === "+");
    }

}

export default RedirectsMediaCondition;