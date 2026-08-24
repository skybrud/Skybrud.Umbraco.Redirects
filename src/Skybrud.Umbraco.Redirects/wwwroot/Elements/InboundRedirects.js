import { LitElement, html, css, when, repeat } from "@umbraco-cms/backoffice/external/lit";

import "@skybrud-redirects/elements/node";
import { RedirectsElementBase } from "@skybrud-redirects/elements/base";

export class InboundRedirectsElement extends RedirectsElementBase {

    constructor() {
        super();
        const self = this;
        self.showTitle = false;
        self.mode = "property-editor";
        this.getCurrentNode(function (node) {
            self.node = node;
            self.requestUpdate();
        });
    }

    render() {
        if (!this.node) return html``;
        return html`<skybrud-redirects-node .node=${this.node} .showTitle=${this.showTitle} .mode=${this.mode}></skybrud-redirects-node>`;
    }

}

customElements.define("redirects-inbound-property-editor", InboundRedirectsElement);

export default InboundRedirectsElement;