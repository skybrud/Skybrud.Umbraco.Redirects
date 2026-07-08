export class RedirectsPackage {

    static set serverVariables(value) {
        this._serverVariables = value;
    }

    static get version() {
        return this._serverVariables["version"];
    }

    static get cacheBuster() {
        return this._serverVariables["cacheBuster"];
    }

    static get settings() {
        return this._serverVariables["settings"];
    }

    static get dashboard() {
        // TODO: ideally we should remove this as the same is exposed via settings.dashboard, but currently not sure if we're still using this anywhere in the codebase, so leaving it for now
        return this._serverVariables["dashboard"] ?? { limit: 20 };
    }

}

export default RedirectsPackage;