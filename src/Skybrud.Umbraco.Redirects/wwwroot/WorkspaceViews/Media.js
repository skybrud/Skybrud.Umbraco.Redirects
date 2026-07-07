import { html, css } from "@umbraco-cms/backoffice/external/lit";

import "@skybrud-redirects/elements/node";
import { RedirectsElementBase } from "@skybrud-redirects/elements/base";

export class RedirectsMediaWorkspaceViewElement extends RedirectsElementBase {

    #node;

    constructor() {
        super();
        const self = this;
        self.showTitle = false;
        self.mode = "workspace-view";
        this.getCurrentNode(function (node) {
            self.node = node;
            self.requestUpdate();
        });
    }

    render() {
        if (!this.node) return html``;
        return html`<skybrud-redirects-node .node=${this.node}></skybrud-redirects-node>`;
    }

    static styles = css`

        :host {
            display: block;
            height: 100%;
        }

    `;

};

customElements.define("redirects-media-workspace-view", RedirectsMediaWorkspaceViewElement);

export default RedirectsMediaWorkspaceViewElement;