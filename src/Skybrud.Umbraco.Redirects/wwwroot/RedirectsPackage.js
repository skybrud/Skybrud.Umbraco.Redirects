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

    static get dashboard() {
        return this._serverVariables["dashboard"] ?? { limit: 20 };
    }

}

export default RedirectsPackage;