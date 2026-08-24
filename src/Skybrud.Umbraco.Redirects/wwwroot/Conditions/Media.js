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

        // Deny access when the current user isn't permitted
        if (!this.#byUser(user)) return false;

        // Check for restrictions based on the media's media type
        if (!this.#byMediaType(mediaType)) return false;

        return true;

    }

    #byMediaType(mediaType) {

        // Allow access when no media type restrictions have been configured
        if (this.#mediaTypes.length === 0) return true;

        // Check for explicit access rules for the current media type
        if (this.#mediaTypes.includes("+" + mediaType.alias)) return true;
        if (this.#mediaTypes.includes("-" + mediaType.alias)) return false;

        // Deny access if an allow-list has been configured, otherwise allow access
        return !this.#mediaTypes.some(x => x.startsWith("+"));

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

export default RedirectsMediaCondition;