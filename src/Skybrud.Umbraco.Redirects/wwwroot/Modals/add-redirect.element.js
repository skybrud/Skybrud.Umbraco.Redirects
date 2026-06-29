import { html, css, repeat, when, unsafeHTML } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

import { RedirectsService } from "@skybrud-redirects/service";
import "@skybrud-redirects/elements/destination";

import { RedirectsModalLoadEvent } from "@skybrud-redirects/events";

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

        this.redirect = {
            redirectType: "permanent",
            forward: false
        };

        RedirectsService.getRootNodes().then(function (res) {

            const temp = [
                { name: self.localize.term("redirects_allSites"), value: "00000000-0000-0000-0000-000000000000", selected: true }
            ];

            res.data.items.forEach(function (rootNode) {
                temp.push({ name: rootNode.name, value: rootNode.key });
            });

            self.rootNodes = temp;

            window.dispatchEvent(new RedirectsModalLoadEvent("redirects.onModalLoad", {
                action: "add",
                redirect: self.redirect
            }));

        });

    }

    connectedCallback() {

        super.connectedCallback();

        // Set the destination, if specified
        if (this.data?.destination) {
            this.redirect.destination = this.data.destination;
        }

        // Set the culture (and list of available cultures), if specified
        if (this.data?.cultures) {
            this.cultures = this.data.cultures;
            this.culture = this.cultures.find(x => x.alias === this.redirect.destination?.culture);
            if (this.culture) this.culture.selected = true;
        }

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
        const culture = this.shadowRoot.querySelector("#culture");

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
            this.errors.push(unsafeHTML(this.localize.term("redirectsErrors_fieldRequired", this.localize.term("redirectsProperties_originalUrl"))));
        } else if (redirect.originalUrl.indexOf("/") !== 0) {
            this.errors.push(unsafeHTML(this.localize.term("redirectsErrors_fieldInvalid", this.localize.term("redirectsProperties_originalUrl"))));
        }

        if (!destination.value) {
            this.errors.push(unsafeHTML(this.localize.term("redirectsErrors_fieldRequired", this.localize.term("redirectsProperties_destination"))));
        } else if (!destination.value.url) {
            this.errors.push(unsafeHTML(this.localize.term("redirectsErrors_fieldInvalid", this.localize.term("redirectsProperties_destination"))));
        }

        if (this.errors.length > 0) {
            console.error("Exiting due to validation errors: ", this.errors);
            this.errors.title = this.localize.term("redirectsErrors_validationErrors");
            self.submitButtonState = "failed";
            self.requestUpdate();
            return;
        }

        destination.value.culture = culture?.value ?? null;

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

    handleDestinationChange(event) {

        const self = this;

        const dest = event.detail?.value;

        if (!dest || !Array.isArray(dest.cultures)) {
            this.redirect.destination = null;
            this.cultures = [];
            self.requestUpdate();
            return;
        }

        const destination = this.shadowRoot.querySelector("#destination");

        RedirectsService.getCultures(dest.key).then(function (res) {
            self.cultures = res.data;
            if (self.cultures.length > 0) self.cultures[0].selected = true;
            self.cultures.forEach(function (culture) {
                culture.value = culture.alias;
            });
            const selected = self.cultures.find(x => x.selected);
            if (selected) destination.setName(selected.nodeName);
            if (selected) destination.setUrl(selected.url);
            self.requestUpdate();
        });

    }

    handleCultureChange(event) {

        const culture = this.cultures?.find(x => x.value === event.target.value);
        if (!culture?.url) return;

        const destination = this.shadowRoot.querySelector("#destination");
        if (!destination) return;

        destination.setName(culture.nodeName);
        destination.setUrl(culture.url);

    }

    render() {
        const self = this;
        function term(key) { return self.localize.term("redirects_" + key); }
        function label(key) { return self.localize.term("redirectsLabels_" + key); }
        function property(key) { return self.localize.term("redirectsProperties_" + key); }
        return html`
            <umb-body-layout headline="${term("addRedirectTitle")}">
                <uui-box>
                    ${when(this.errors?.length > 0, () => html`
                        <div class="errors">
                            <div>${this.errors.title}</div>
                            <ul>
                                ${this.errors.map(error => html`<li>${error}</li>`)}
                            </ul>
                        </div>
                    `)}
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
                            <uui-input id="originalUrl" label="${property("originalUrl")}"></uui-input>
                        </div>
                    </div>
                    <div class="property">
                        <div>
                            <strong>${property("destination")}<span style="color: red;">*</span></strong><br />
                            <small>${property("destinationDescription")}</small>
                        </div>
                        <div>
                            <redirects-destination id="destination" .value=${this.redirect.destination} @change="${(e) => this.handleDestinationChange(e)}"></redirects-destination>
                        </div>
                    </div>
                    ${when(this.cultures?.length > 1, () => html`
                        <div class="property">
                            <div>
                            <strong>${property("destinationCulture")}</strong><br />
                            <small>${property("destinationCultureDescription")}</small>
                            </div>
                            <div>
                                <uui-select id="culture" label="Culture" .options=${this.cultures} @change=${(e) => this.handleCultureChange(e)}>
                                    ${repeat(this.cultures, (item) => item.key, (item) => html`
                                        <option value="${item.value}">${item.name}</option>
                                    `)}
                                </uui-select>
                            </div>
                        </div>
                    `)}
                    <h4>${label("advancedOptions")}</h4>
                    <div class="property">
                        <div>
                            <strong>${property("redirectType")}</strong><br />
                            <small>${property("redirectTypeDescription")}</small>
                        </div>
                        <div>
                            <uui-radio-group name="redirectType" value="${this.redirect.redirectType}">
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
                            <uui-radio-group name="forward" value="${this.redirect.forward}">
                                <uui-radio id="forwardEnabled" value="true" label="${term("enabled")}"></uui-radio>
                                <uui-radio id="forwardDisabled" value="false" label="${term("disabled")}"></uui-radio>
                            </uui-radio-group>
                        </div>
                    </div>
                </uui-box>
                <div slot="actions">
                        <uui-button id="cancel" label="${this.localize.term("general_cancel")}" @click="${this.handleCancel}">${this.localize.term("general_cancel")}</uui-button>
                        <uui-button
                            id="submit"
                            color='positive'
                            look="primary"
                            label="${term("save")}"
                            state="${this.submitButtonState}"
                            @click="${this.handleConfirm}"></uui-button>
                </div>
            </umb-body-layout>
        `;
    }

    static styles = css`

        .errors {
            margion-bottom: 20px;
            color: var(--uui-color-danger);
            > div {
                font-weight: bold;
            }
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
        uui-select {
            width: 100%;
        }
    `;

}

customElements.define("redirects-add-redirect", AddRedirectModelElement);

export default AddRedirectModelElement;