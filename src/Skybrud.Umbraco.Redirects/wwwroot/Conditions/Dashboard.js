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

export default RedirectsDashboardCondition;