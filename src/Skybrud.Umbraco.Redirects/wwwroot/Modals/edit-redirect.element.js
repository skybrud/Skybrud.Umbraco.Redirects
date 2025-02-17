import { html, css, when, repeat } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

import { RedirectsService } from "@skybrud-redirects/service";
import "@skybrud-redirects/elements/destination";

export class EditRedirectModalElement extends UmbModalBaseElement {

    get tab() {
        return this._tab;
    }

    set tab(value) {
        this._tab = value;
        this.requestUpdate();
    }

    get rootNodes() {
        return this._rootNodes;
    }

    set rootNodes(value) {
        this._rootNodes = value;
        this.requestUpdate();
    }

    constructor() {

        super();

        this._tab = "settings";

        const self = this;

        this.submitButtonState = null;

        this.rootNodes = [
            { name: self.localize.term("redirects_allSites"), value: "00000000-0000-0000-0000-000000000000", selected: true }
        ];

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this._modalManagerContext = instance;
        });

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
            this._notificationContext = instance;
        });

    }

    connectedCallback() {

        super.connectedCallback();

        const self = this;

        // Create a deep copy of the redirect model so that we can modify it (not sure why we can't otherwise)
        const redirect = JSON.parse(JSON.stringify(this.value.redirect ?? {}));

        // Set fallback values
        if (!redirect.type) this.value.redirect.type = "permanent";
        if (!redirect.forward) redirect.forward = false;

        // Update the component value
        this.updateValue({
            title: redirect?.key ? self.localize.term("redirects_editRedirectTitle") : self.localize.term("redirects_addRedirectTitle"),
            redirect: redirect
        });

        RedirectsService.getRootNodes().then(function (res) {

            const temp = [
                { name: self.localize.term("redirects_allSites"), value: "00000000-0000-0000-0000-000000000000" }
            ];

            res.data.items.forEach(function (rootNode) {
                temp.push({ name: rootNode.name, value: rootNode.key });
            });

            if (redirect.rootNode) {
                const selected = temp.find(x => x.value === redirect.rootNode.key);
                if (selected) {
                    selected.selected = true;
                } else {
                    temp[0].selected = true;
                }
            } else {
                temp[0].selected = true;
            }

            self.rootNodes = temp;

        });


    }

    handleCancel() {
        this.modalContext?.reject();
    }

    handleConfirm() {

        const self = this;

        const rootNode = this.shadowRoot.querySelector("#rootNode");
        const originalUrl = this.shadowRoot.querySelector("#originalUrl");
        const destination = this.shadowRoot.querySelector("#destination");
        const redirectTypePermanent = this.shadowRoot.querySelector("#redirectTypePermanent");
        const forwardEnabled = this.shadowRoot.querySelector("#forwardEnabled");

        const redirect = {
            id: this.value.redirect.id,
            key: this.value.redirect.key,
            rootNodeKey: rootNode.value,
            originalUrl: originalUrl.value,
            destination: destination.value,
            permanent: redirectTypePermanent.checked,
            forward: forwardEnabled.checked
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

        if (this.errors.length > 0) {
            console.log(redirect, this.errors);
            self.submitButtonState = "failed";
            self.requestUpdate();
            return;
        }

        self.submitButtonState = "waiting";
        self.requestUpdate();

        RedirectsService.saveRedirect(redirect).then(function (res) {
            self.updateValue({ title: self.value.title, redirect: res.data });
            self.modalContext?.submit(res.data);
        }, function (res) {
            self.submitButtonState = "failed";
            self._notificationContext?.peek("danger", {
                data: {
                    message: "Saving redirect failed" + (res.data?.error ? ":\r\n" + res.data?.error : ".")
                }
            });
            self.requestUpdate();
        });

    }

    changeTab(alias) {
        this.tab = alias;
        this.requestUpdate();
    }

    renderInfo() {

        const redirect = this.value.redirect;
        if (!redirect) return "";

        const createDate = new Date(this.value.redirect.createDate);
        const updateDate = new Date(this.value.redirect.updateDate);

        const dateOptions = {
            day: "numeric",
            hour: "numeric",
            minute: "numeric",
            month: "long",
            second: "numeric",
            year: "numeric"
        };

        return html`
            <uui-box>
                <div class="property">
                    <div>
                        <strong>${this.localize.term("redirectsProperties_id")}</strong><br />
                    </div>
                    <div>
                        <code>${this.value.redirect?.id}</code>
                    </div>
                </div>
                <div class="property">
                    <div>
                        <strong>${this.localize.term("redirectsProperties_key")}</strong><br />
                    </div>
                    <div>
                        <code>${this.value.redirect?.key}</code>
                    </div>
                </div>
                <div class="property">
                    <div>
                        <strong>${this.localize.term("redirectsProperties_createDate")}</strong><br />
                    </div>
                    <div>
                        ${this.localize.date(createDate, dateOptions)}
                        <small>(<limbo-from-now>${this.value.redirect.createDate}</limbo-from-now>)</small>
                    </div>
                </div>
                <div class="property">
                    <div>
                        <strong>${this.localize.term("redirectsProperties_updateDate")}</strong><br />
                    </div>
                    <div>
                        ${this.localize.date(updateDate, dateOptions)}
                        <small>(<limbo-from-now>${this.value.redirect.updateDate}</limbo-from-now>)</small>
                    </div>
                </div>
            </uui-box>
            `;

    }

    render() {
        const self = this;
        function term(key) { return self.localize.term("redirects_" + key); }
        function label(key) { return self.localize.term("redirectsLabels_" + key); }
        function property(key) { return self.localize.term("redirectsProperties_" + key); }
        return html`
            <umb-body-layout headline="${this.value.title}" style="position: relative;">
                ${when(this.value.redirect?.key, () => html`
                    <div id="hest" style="position: absolute; right: 0; top: 0; height: 70px; z-index: 9001;">
                        <uui-tab-group>
                            <uui-tab @click="${() => this.changeTab("settings")}" label="${this.localize.term("redirectsTabs_settings")}" active>
                                <uui-icon slot="icon" name="settings"></uui-icon>
                                ${this.localize.term("redirectsTabs_settings")}
                            </uui-tab>
                            <uui-tab @click="${() => this.changeTab("info")}" label="${this.localize.term("redirectsTabs_info")}">
                                <uui-icon slot="icon" name="info"></uui-icon>
                                ${this.localize.term("redirectsTabs_info")}
                            </uui-tab>
                        </uui-tab-group>
                    </div>
                `)}
                ${when(this.tab === "settings", () => html`
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
                                <strong>${property("originalUrl")}<span style="color: red;">*</span></strong><br />
                                <small>${property("originalUrlDescription")}</small>
                            </div>
                            <div>
                                <uui-input id="originalUrl" label="${property("originalUrl")}" value="${this.value.redirect?.url}"></uui-input>
                            </div>
                        </div>
                        <div class="property">
                            <div>
                                <strong>${property("destination")}<span style="color: red;">*</span></strong><br />
                                <small>${property("destinationDescription")}</small>
                            </div>
                            <div>
                                <redirects-destination id="destination">${JSON.stringify(this.value.redirect?.destination)}</redirects-destination>
                            </div>
                        </div>
                        <h4>${label("advancedOptions")}</h4>
                        <div class="property">
                            <div>
                                <strong>${property("redirectType")}</strong><br />
                                <small>${property("redirectTypeDescription")}</small>
                            </div>
                            <div>
                                <uui-radio-group name="redirectType" value="${this.value.redirect.type}">
                                    <uui-radio id="redirectTypePermanent" value="permanent" label="${term("permanent")}"></uui-radio>
                                    <uui-radio id="redirectTypeTemporary" value="temporary" label="${term("temporary")}"></uui-radio>
                                </uui-radio-group>
                            </div>
                        </div>
                        <div class="property">
                            <div>
                                <strong>${property("forwardQueryString")}</strong><br />
                                <small>${property("forwardQueryStringDescription")}</small>
                            </div>
                            <div>
                                <uui-radio-group name="forward" value="${this.value.redirect.forward}">
                                    <uui-radio id="forwardEnabled" value="true" label="${term("enabled")}"></uui-radio>
                                    <uui-radio id="forwardDisabled" value="false" label="${term("disabled")}"></uui-radio>
                                </uui-radio-group>
                            </div>
                        </div>
                    </uui-box>
                `)}
                ${when(this.tab === "info", () => this.renderInfo())}
                <div slot="actions">
                    <uui-button id="cancel" label="${this.localize.term("general_cancel")}" @click="${this.handleCancel}"></uui-button>
                    <uui-button id="save" color="positive" look="primary" label="${term("save")}" ?disabled="${this.tab === "info"}" state="${this.submitButtonState}" ${this.tab === "info" ? "disabled" : ""} @click="${this.handleConfirm}"></uui-button>
                </div>
            </umb-body-layout>
        `;
    }

    static styles = css`
        uui-tab {
            border-left: 1px solid var(--uui-color-divider-standalone);
        }
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
        code {
            border-radius: 3px;
            color: #1b264f;
            font-family: Monaco, Menlo, Consolas, Courier New, monospace;
            font-size: 13px;
            padding: 0 3px 2px;
            background-color: #f7f7f9;
            border: 1px solid #e1e1e8;
            padding: 2px 4px;
            white-space: nowrap;
        }
        uui-select {
            width: 100%;
        }
    `;

}

customElements.define("redirects-edit-redirect", EditRedirectModalElement);

export default EditRedirectModalElement;