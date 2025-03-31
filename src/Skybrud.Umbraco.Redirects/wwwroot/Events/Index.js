export class RedirectsDashboardLoadEvent extends Event {
    constructor(evName, eventInit = {}) {
        super(evName, { ...eventInit })
        this.dashboard = eventInit.dashboard || {};
    }
}

export class RedirectsModalLoadEvent extends Event {
    constructor(evName, eventInit = {}) {
        super(evName, { ...eventInit })
        this.action = eventInit.action || "";
        this.redirect = eventInit.redirect ?? {};
        this.tabs = Array.isArray(eventInit.tabs) ? eventInit.tabs : [];
    }
}