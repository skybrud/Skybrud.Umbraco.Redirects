import { html, css, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

import { RedirectsService } from "@skybrud-redirects/service";
import "@skybrud-redirects/elements/destination";

export class AddRedirectModelElement extends UmbModalBaseElement {

    get rootNodes() {
        return this._rootNodes;
    }

    set rootNodes(value) {
        this._rootNodes = value;
        this.requestUpdate();
    }

    constructor() {

        super();

        const self = this;

        this.rootNodes = [
            { name: self.localize.term("redirects_allSites"), value: "00000000-0000-0000-0000-000000000000", selected: true }
        ];

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this._modalManagerContext = instance;
        });

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
            this._notificationContext = instance;
        });

        RedirectsService.getRootNodes().then(function (res) {

            const temp = [
                { name: self.localize.term("redirects_allSites"), value: "00000000-0000-0000-0000-000000000000", selected: true }
            ];

            res.data.items.forEach(function (rootNode) {
                temp.push({ name: rootNode.name, value: rootNode.key });
            });

            self.rootNodes = temp;

        });

    }


    handleCancel() {
        this.modalContext?.reject();
    }

    handleConfirm() {

        const self = this;

        if (self.submitButtonState === "waiting") {
            console.log("Ignoring submit button click as underlying logic hasn't finished yet...");
            return;
        }

        const rootNode = this.shadowRoot.querySelector("#rootNode");
        const originalUrl = this.shadowRoot.querySelector("#originalUrl");
        const destination = this.shadowRoot.querySelector("#destination");

        const redirectTypeTemporary = this.shadowRoot.querySelector("#redirectTypeTemporary");

        const forwardEnabled = this.shadowRoot.querySelector("#forwardEnabled");

        const redirect = {
            rootNodeKey: rootNode.value && rootNode.value != "all" ? rootNode.value : null,
            originalUrl: originalUrl.value,
            destination: destination.value,
            type: redirectTypeTemporary.checked ? "temporary" : "permanent",
            forwardQueryString: forwardEnabled.checked
        };

        this.errors = [];

        if (!redirect.originalUrl) {
            this.errors.push("Original URL not specified.");
        } else if (redirect.originalUrl.indexOf("/") !== 0) {
            this.errors.push("Invalid original URL.");
        }

        if (!destination.value) {
            this.errors.push("Destination not specified.");
            return;
        } else if (!destination.value.url) {
            this.errors.push("Destination URL not specified.");
            return;
        }

        if (this.errors.length > 0) return;

        self.submitButtonState = "waiting";
        self.requestUpdate();

        RedirectsService.addRedirect(redirect).then(function (res) {
            self.submitButtonState = "success";
            self.updateValue({ redirect: res.data });
            self.modalContext?.submit(res.data);
            self.requestUpdate();
        }, function (res) {
            self.submitButtonState = "failed";
            self._notificationContext?.peek("danger", {
                data: {
                    message: "Adding redirect failed" + (res.data?.error ? ": " + res.data?.error : ".")
                }
            });
            self.requestUpdate();
        });


    }

    render() {
        return html`
            <umb-body-layout headline="Add new redirect">
                <uui-box>
                    <div class="property">
                        <div>
                            <strong>${this.localize.term("redirectsProperties_site")}</strong><br />
                            <small>${this.localize.term("redirectsProperties_siteDescription")}</small>
                        </div>
                        <div>
                            <uui-select id="rootNode" label="Root node" .options=${this.rootNodes}>
                                ${repeat(this.rootNodes, (item) => item.key, (item) => html`
                                    <option value="${item.value}">${item.name}</option>
                                `)}
                            </uui-select>
                        </div>
                    </div>
                    <div class="property">
                        <div>
                            <strong>Original URL<span style="color: red;">*</span></strong><br />
                            <small>Specify the original URL to match from which the user should be redirected to the destination.</small>
                        </div>
                        <div>
                            <uui-input id="originalUrl" label="Original URL"></uui-input>
                        </div>
                    </div>
                    <div class="property">
                        <div>
                            <strong>Destination<span style="color: red;">*</span></strong><br />
                            <small>Select the page or URL the user should be redirected to.</small>
                        </div>
                        <div>
                            <redirects-destination id="destination"></redirects-destination>
                        </div>
                    </div>
                    <h4>Advanced Options</h4>
                    <div class="property">
                        <div>
                            <strong>Redirect type</strong><br />
                            <small>Select the type of the redirect. Notice that browsers will remember permanent redirects.</small>
                        </div>
                        <div>
                            <uui-radio-group name="redirectType">
                                <uui-radio id="redirectTypePermanent" value="permanent" label="Permanent" checked="checked"></uui-radio>
                                <uui-radio id="redirectTypeTemporary" value="temporary" label="Temporary"></uui-radio>
                            </uui-radio-group>
                        </div>
                    </div>
                    <div class="property">
                        <div>
                            <strong>Forward query string</strong><br />
                            <small>When enabled, the query string of the original request is forwarded to the redirect location (pass through).</small>
                        </div>
                        <div>
                            <uui-radio-group name="forward">
                                <uui-radio id="forwardEnabled" value="enabled" label="Enabled"></uui-radio>
                                <uui-radio id="forwardDisabled" value="disabled" label="Disabled" checked="true"></uui-radio>
                            </uui-radio-group>
                        </div>
                    </div>
                </uui-box>
                <div slot="actions">
                        <uui-button id="cancel" label="Cancel" @click="${this.handleCancel}">Cancel</uui-button>
                        <uui-button
                            id="submit"
                            color='positive'
                            look="primary"
                            label="Submit"
                            state="${this.submitButtonState}"
                            @click="${this.handleConfirm}"></uui-button>
                </div>
            </umb-body-layout>
        `;
    }

    static styles = css`
        .property + .property {
            margin-top: 20px;
            border-top: 1px solid var(--uui-color-divider);
            padding-top: 20px;
        }
        .property uui-input {
            width: 100%;
        }
        uui-radio-group {
            display: flex;
            gap: 25px;
        }
        h4 {
            font-size: 18px;
            margin: 75px 0 15px 0;
            border-bottom: 1px solid var(--uui-color-border);
            padding-bottom: 10px;
            font-weight: bold;
        }
        uui-select {
            width: 100%;
        }
    `;

}

customElements.define("redirects-add-redirect", AddRedirectModelElement);

export default AddRedirectModelElement;