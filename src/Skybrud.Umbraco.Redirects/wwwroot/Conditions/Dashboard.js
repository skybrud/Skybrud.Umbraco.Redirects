import { UmbConditionBase } from "@umbraco-cms/backoffice/extension-registry";

import { RedirectsPackage } from "@skybrud-redirects/package";
import { RedirectsService } from "@skybrud-redirects/service";

export class RedirectsDashboardCondition extends UmbConditionBase {

    #userGroups;

    constructor(host, args) {
        super(host, args);
        this.#userGroups = RedirectsPackage.settings?.dashboard?.userGroups ?? [];
        RedirectsService.getCurrentUser().then((user) => {
            this.permitted = this.#callback(user);
            args.onChange();
        });
    }

    #callback(user) {

        // Allow access when no restrictions have been configured
        if (this.#userGroups.length === 0) return true;

        // Check for explicit user access
        if (this.#userGroups.includes("+" + user.key)) return true;
        if (this.#userGroups.includes("-" + user.key)) return false;

        // Check for access based on the user's groups
        if (user.groups.some(group => this.#userGroups.includes("+" + group))) return true;
        if (user.groups.some(group => this.#userGroups.includes("-" + group))) return false;

        // Deny access if an allow-list has been configured, otherwise allow access
        return !this.#userGroups.some(x => x.startsWith("+"));

    }

}

export default RedirectsDashboardCondition;