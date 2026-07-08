import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, when, repeat } from "@umbraco-cms/backoffice/external/lit";

import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';

import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

import { REDIRECTS_ADD_REDIRECT_MODAL } from "@skybrud-redirects/modals/add";
import { REDIRECTS_EDIT_REDIRECT_MODAL } from "@skybrud-redirects/modals/edit";

import { RedirectsService } from "@skybrud-redirects/service";

export class SkybrudRedirectsNodeElement extends UmbElementMixin(LitElement) {

    #loading;
    #error;
    #node;
    #redirects;
    #modalManagerContext;
    #notificationContext;
    #list;
    #showTitle;
    #mode;

    get showTitle() {
        return this.#showTitle ?? true;
    }

    set showTitle(value) {
        this.#showTitle = value !== false;
        this.requestUpdate();
    }

    get mode() {
        return this.#mode;
    }

    set mode(value) {
        this.#mode = value;
        this.requestUpdate();
    }

    get node() {
        return this.#node;
    }

    set node(value) {
        this.#error = null;
        this.#redirects = null;
        this.#node = value;
        this.#showTitle = true;
        if (value) {
            if (value.new) {
                this.#error = { type: "info", content: "You are in the process of creating this page, meaning you cannot yet add redirects to it. Save the page to continue, then you'll be able to add redirects." };
            } else if (value.trashed) {
                this.#error = { type: "warning", content: "This page is current trashed. Any redirect pointing to this page will not work." };
            } else if (value.published === false) {
                this.#error = { type: "warning", content: "This page is currently not published, meaning redirects pointing to it may not work." };
            }
            if (!value.new) this.#loadRedirects();
        }
        this.requestUpdate();
    }

    constructor() {

        super();

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.#modalManagerContext = instance;
        });

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
            this.#notificationContext = instance;
        });

    }

    async #loadRedirects() {

        const self = this;

        this.#loading = true;
        this.requestUpdate();

        if (!this.node) {
            this.#redirects = [];
            return;
        }

        const [response] = await Promise.all([
            RedirectsService.getRedirectsForNode(this.node.type, this.node.key),
            new Promise(resolve => setTimeout(resolve, 200))
        ]);

        this.#list = {
            reload: () => this.reload(),
            actions: [
                {
                    alias: "add",
                    label: this.localize.term("redirects_addRedirect"),
                    action: function () {
                        self.#add();
                    },
                    look: "primary",
                    color: "positive",
                    subButtons: []
                },
                {
                    alias: "reload",
                    label: this.localize.term("redirects_reload"),
                    action: function () {
                        self.#reload();
                    },
                    look: "outline",
                    color: "default",
                    subButtons: []
                }
            ]
        };

        this.#loading = false;
        this.#redirects = response.data.items;
        this.requestUpdate();

    }

    #reload() {
        this.#loadRedirects();
    }

    #add() {

        const self = this;

        const modalContext = this.#modalManagerContext?.open(this, REDIRECTS_ADD_REDIRECT_MODAL, {
            data: {
                destination: this.#node,
                cultures: this.#node?.__cultures ?? []
            }
        });

        modalContext.onSubmit().then(function () {
            self.#loadRedirects();
            self.#notificationContext?.peek("positive", { data: { message: self.localize.term("redirects_addRedirectSuccess") } });
        }, function () {
            // modal closed by the user
        });

    }

    #edit(redirect) {

        const self = this;

        const modalContext = this.#modalManagerContext.open(this, REDIRECTS_EDIT_REDIRECT_MODAL, {
            redirect,
            value: {
                redirect
            }
        });

        modalContext.onSubmit().then(function () {
            self.#loadRedirects();
            self.#notificationContext?.peek("positive", { data: { message: self.localize.term("redirects_editRedirectSuccess") } });
        }, function () {
            // modal closed by the user
        });

    }

    #delete(redirect) {

        const self = this;

        if (!redirect?.key) return;
        umbConfirmModal(this, {
            headline: this.localize.term("redirects_deleteRedirectTitle"),
            content: html`
				<div style="width:500px">
					<p>${this.localize.term("redirects_deleteRedirectMessage")}</p>
					${this.localize.term("redirects_originalUrl")}: <strong>${redirect.url}</strong><br />
					${this.localize.term("redirects_destination")}: <strong>${redirect.destination.url}</strong>
				</div>
			`,
            color: 'danger',
            confirmLabel: this.localize.term("general_delete"),
        }).then(function () {
            RedirectsService.deleteRedirect(redirect).then(function () {
                self.#notificationContext?.peek("positive", { data: { message: self.localize.term("redirects_deleteRedirectSuccess") } });
                self.#loadRedirects();
            }, function () {
                self.#notificationContext?.peek("danger", { data: { message: self.localize.term("redirects_deleteRedirectFailed") } });
            });
        }, function () {
            // model was cancelled
        });

    }

    #renderNoRedirects() {

        if (this.text || this.rootNode || this.type) {
            return html`
                <div class="umb-empty-state -center">
                    ${this.localize.term("redirectsLabels_noSearchRedirects")}
                </div>
            `;
        }

        return html`
            <div class="umb-empty-state -center">
                ${this.localize.term("redirectsLabels_noRedirects")}
            </div>
        `;

    }

    renderDestinationType(r) {

        return html`
            ${when(r.destination.name, () => html`
                ${when(r.destination.culture, () => html`
                    <small title="${this.localize.term("redirectsProperties_destinationCulture")}: ${r.destination.cultureName}">
                        <uui-icon name="icon-flag-alt"></uui-icon>
                        ${r.destination.culture}
                    </small>
                `)}
            `, () => html`
                ${type}
            `)}
        `;

    }

    #renderTable() {
        if (!this.#redirects?.length > 0) return html``;
        return html`
            <uui-table role="table">
                <uui-table-head role="row">
                    <uui-table-head-cell role="columnheader">${this.localize.term("redirects_site")}</uui-table-head-cell>
                    <uui-table-head-cell role="columnheader" style="min-width: 250px;">${this.localize.term("redirects_originalUrl")}</uui-table-head-cell>
                    <uui-table-head-cell role="columnheader">${this.localize.term("redirects_type")}</uui-table-head-cell>
                    <uui-table-head-cell role="columnheader"></uui-table-head-cell>
                    <uui-table-head-cell role="columnheader" style="width: 100%;">${this.localize.term("redirects_destination")}</uui-table-head-cell>
                    <uui-table-head-cell role="columnheader"></uui-table-head-cell>
                </uui-table-head>
                ${repeat(this.#redirects, (item) => item.key, (item) => html`
                    <uui-table-row role="row">
                        <uui-table-cell role="cell" class="col-root-node">
                            ${when(item.rootNode, () => html`
                                <a href="${item.rootNode.backOfficeUrl}">${item.rootNode.name}</a>
                            `)}
                            ${when(!item.rootNode, () => html`
                                <span style="white-space: nowrap;">${this.localize.term("redirects_allSites")}</span>
                            `)}
                        </uui-table-cell>
                        <uui-table-cell role="cell" class="col-url">
                            <a href="${item.fullUrl}" rel="noreferrer" target="_blank">${item.url}</a>
                            ${when(item.urlWarning, () => html`
                                <small class=\"warning\">${this.localize.term("redirects_" + item.urlWarning)}</small>
                            `)}
                        </uui-table-cell>
                        <uui-table-cell role="cell">
                            ${when(item.type === "permanent", () => this.localize.term("redirects_permanent"))}
                            ${when(item.type === "temporary", () => this.localize.term("redirects_temporary"))}
                        </uui-table-cell>
                        <uui-table-cell role="cell">
                            <uui-icon name="icon-arrow-right" aria-hidden="true"></uui-icon>
                        </uui-table-cell>
                        <uui-table-cell role="cell">
                            ${this.renderDestinationType(item)}
                            ${when(item.forward, () => html`
                                <small class="forward" title="Forward query string is enabled">&nbsp;?&amp;</small>
                            `)}
                            <div class="displayUrl">
                                <a href="${item.destination.displayUrl}" rel="noreferrer" target="_blank">${item.destination.displayUrl}</a>
                            </div>
                        </uui-table-cell>
                        <uui-table-cell class="actions" role="cell">
                            <uui-action-bar>
                                <uui-button label="Edit" title="${this.localize.term("redirects_editRedirectTitle")}" look="secondary" pristine="" type="button" color="default" @click="${() => this.#edit(item)}">
                                    <uui-icon name="edit" aria-hidden="true"></uui-icon>
                                </uui-button>
                                <uui-button label="Delete" title="${this.localize.term("redirects_deleteRedirectTitle")}" look="secondary" pristine="" type="button" color="danger" @click="${() => this.#delete(item)}">
                                    <uui-icon name="delete" aria-hidden="true"></uui-icon>
                                </uui-button>
                            </uui-action-bar>
                        </uui-table-cell>
                    </uui-table-row>
                `)}
            </uui-table>
        `;
    }

    #renderTableWrapper() {

        if (this.mode === "property-editor") {
            return html`
                <div class="redirects">
                    ${this.#renderTable()}
                </div>
            `;
        }

        if (this.mode === "info-app") {
            return html`

                <uui-box headline="Redirects">
                    <div slot="header-actions">
                        ${when(this.#list?.actions, () => html`
                            ${repeat(this.#list.actions, (button) => button.alias, (button) => html`
                                <uui-button-group>
                                    <uui-button look="${button.look}" color="${button.color}" label="${button.label}" state="${button.state}" @click=${button.action}>
                                        ${button.label}
                                    </uui-button>
                                    ${when(button.subButtons?.length > 0, () => html`
                                        <uui-button popovertarget="my-popover" look="${button.look}" color="${button.color}">
                                            <uui-symbol-more></uui-symbol-more>
                                        </uui-button>
                                        <uui-popover-container id="my-popover" placement="bottom-end">
                                            <div style="display: flex; flex-direction: column;">
                                                ${repeat(button.subButtons, (sub) => sub.alias, (sub) => html`
                                                    <uui-button look="${sub.look}" label="${sub.label}" @click=${sub.action}>
                                                        ${sub.label}
                                                    </uui-button>
                                                `)}
                                            </div>
                                        </uui-popover-container>
                                    `)}
                                </uui-button-group>
                            `)}
                        `)}
                    </div>
                </uui-box>

                <div class="redirects">
                    ${this.#renderTable()}
                </div>
            `;
        }

        return html`
            <div class="redirects">
                ${this.#renderTable()}
            </div>
        `;

    }

    #renderActions() {
        if (!this.#list?.actions) return html``;
        return html`
            ${repeat(this.#list.actions, (button) => button.alias, (button) => html`
                <uui-button-group>
                    <uui-button look="${button.look}" color="${button.color}" label="${button.label}" state="${button.state}" @click=${button.action}>
                        ${button.label}
                    </uui-button>
                    ${when(button.subButtons?.length > 0, () => html`
                        <uui-button popovertarget="my-popover" look="${button.look}" color="${button.color}">
                            <uui-symbol-more></uui-symbol-more>
                        </uui-button>
                        <uui-popover-container id="my-popover" placement="bottom-end">
                            <div style="display: flex; flex-direction: column;">
                                ${repeat(button.subButtons, (sub) => sub.alias, (sub) => html`
                                    <uui-button look="${sub.look}" label="${sub.label}" @click=${sub.action}>
                                        ${sub.label}
                                    </uui-button>
                                `)}
                            </div>
                        </uui-popover-container>
                    `)}
                </uui-button-group>
            `)}
        `;
    }

    #renderErrors() {
        if (!this.#error) return html``;
        return html`
            <div class="alert alert--warning">
                <uui-icon name="icon-alert"></uui-icon>
                <div>
                    ${when(this.#error.title, () => html`
                        <strong>${this.#error.title}</strong>
                    `)}
                    <p>${this.#error.content}</p>
                </div>
            </div>
        `;
    }

    #renderInfoApp() {
        return html`
            <div class="container ${this.#loading ? "loading" : ""} ${this.mode}">
                <uui-box headline="Redirects">
                    <div slot="header-actions">
                        ${this.#renderActions()}
                    </div>
                    ${this.#renderErrors()}
                    <div class="redirects">
                        ${this.#renderTable()}
                    </div>
                    ${when(this.#loading, () => html`<uui-loader></uui-loader>`)}
                    ${when(this.#redirects?.length === 0, () => this.#renderNoRedirects())}
                </uui-box>
            </div>
        `;
    }

    #renderPropertyEditor() {
        return html`
            <div class="container ${this.#loading ? "loading" : ""} ${this.mode}">
                <header>
                    ${when(this.showTitle, () => html`<h3>Redirects</h3>`)}
                    <div class="actions">
                        ${this.#renderActions()}
                    </div>
                </header>
                ${this.#renderErrors()}
                ${when(this.#redirects?.length > 0, () => html`
                    <div class="redirects">
                        ${this.#renderTable()}
                    </div>
                `)}
                ${when(this.#loading, () => html`<uui-loader></uui-loader>`)}
                ${when(this.#redirects?.length === 0, () => this.#renderNoRedirects())}
            </div>
        `;

    }

    #renderWorkspaceView() {
        return html`
            <div class="container ${this.#loading ? "loading" : ""} ${this.mode}">
                <header>
                    ${when(this.showTitle, () => html`<h3>Redirects</h3>`)}
                    <div class="actions">
                        ${this.#renderActions()}
                    </div>
                </header>
                ${this.#renderErrors()}
                ${when(this.#redirects?.length > 0, () => html`
                    <uui-box style="--uui-box-default-padding:0;">
                        ${this.#renderTable()}
                    </uui-box>
                `)}
                ${when(this.#loading, () => html`<uui-loader></uui-loader>`)}
                ${when(this.#redirects?.length === 0, () => this.#renderNoRedirects())}
            </div>
        `;

    }

    render() {
        if (this.mode === "info-app") return this.#renderInfoApp();
        if (this.mode === "property-editor") return this.#renderPropertyEditor();
        return this.#renderWorkspaceView();
    }

    static styles = css`

        :host {
            display: block;
            height: 100%;
            position: relative;
        }

        .container {
            display: flex;
            flex-direction: column;
            gap: 20px;
            padding: 20px;
        }

        .container.info-app {
            padding: 0;
            uui-box {
                --uui-box-default-padding: 0;
            }
        }

        .container.property-editor {
            padding: 0;
            uui-table {
                border: 1px solid var(--uui-color-border);
            }
        }

        uui-loader {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
        }

        .loading .redirects,
        .loading .umb-empty-state {
            opacity: 0.6;
            pointer-events: none
        }

        header {
            display: flex;
            gap: 10px;
            align-items: center;
        }

        header > h3 {
            flex: 1;
            margin: 0;
        }

        .actions {
            display: flex;
            gap: 10px;
            margin-left: auto;
        }

        .alert {
            display: flex;
            gap: 1rem;
            padding: 1rem;
            border-left: 4px solid;
            border-radius: var(--uui-border-radius);
            p:first-child {
                margin-top: 0;
            }
            p:last-child {
                margin-bottom: 0;
            }
        }

        .alert--info {
            border-color: var(--uui-color-info);
            background: color-mix(in srgb, var(--uui-color-info) 8%, white);
        }

        .alert--success {
            border-color: var(--uui-color-positive);
            background: color-mix(in srgb, var(--uui-color-positive) 8%, white);
        }

        .alert--warning {
            border-color: var(--uui-color-warning);
            background: color-mix(in srgb, var(--uui-color-warning) 8%, white);
        }

        .alert--danger {
            border-color: var(--uui-color-danger);
            background: color-mix(in srgb, var(--uui-color-danger) 8%, white);
        }

        uui-table-head-cell {
            padding-top: 5px;
            padding-bottom: 5px;
        }

        uui-table-cell {
            padding-top: 5px;
            padding-bottom: 5px;
        }

        a {
            color: var(--uui-color-interactive);
        }

        uuuuuuuuuui-table-cell.col-url {
            width: 100%;
        }

        uui-table-cell.actions {
            display: flex;
            justify-content: flex-end;
        }

        .umb-empty-state {
            color: #68676b;
            font-size: 17.25px;
            line-height: 1.8em;
            text-align: center;
        }

        .umb-empty-state.-center {
            left: 50%;
            max-width: 400px;
            position: absolute;
            top: 50%;
            transform: translate(-50%, -50%);
            width: 80%;
        }

    `;

}

customElements.define("skybrud-redirects-node", SkybrudRedirectsNodeElement);

export default SkybrudRedirectsNodeElement;