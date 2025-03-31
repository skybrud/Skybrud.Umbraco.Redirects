import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";

import '@umbraco-cms/backoffice/components';

import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

import { RedirectsService } from "@skybrud-redirects/service";
import { REDIRECTS_ADD_REDIRECT_MODAL } from "@skybrud-redirects/modals/add";
import { REDIRECTS_EDIT_REDIRECT_MODAL } from "@skybrud-redirects/modals/edit";

import { RedirectsDashboardLoadEvent } from "@skybrud-redirects/events";

function ucfirst(value) {
    return String(value).charAt(0).toUpperCase() + String(value).slice(1);
}

export class RedirectsDashboardElement extends UmbElementMixin(LitElement) {

    get text() {
        return this._text;
    }

    set text(value) {
        this._text = value;
        this.updateRedirects(1);
    }

    get loading() {
        return this._loading;
    }

    set loading(value) {
        this._loading = value;
        this.requestUpdate();
    }

    get refreshing() {
        return this._refreshing;
    }

    set refreshing(value) {
        this._refreshing = value;
        this.requestUpdate();
    }

    get pagination() {
        return this._pagination;
    }

    set pagination(value) {
        this._pagination = value;
    }

    get redirects() {
        return this._redirects;
    }

    set redirects(value) {
        this._redirects = value;
        this.requestUpdate();
    }

    get rootNodes() {
        return this._rootNodes;
    }

    set rootNodes(value) {
        this._rootNodes = value;
    }

    constructor() {

        super();

        const self = this;

        this.rootNodes = [
            { name: self.localize.term("redirects_allSites"), value: "all", selected: true },
            { name: self.localize.term("redirects_globalRedirects"), value: "00000000-0000-0000-0000-000000000000" }
        ];

        this.types = [
            { name: this.localize.term("redirects_allTypes"), value: "all", selected: true },
            { name: this.localize.term("redirects_content"), value: "content" },
            { name: this.localize.term("redirects_media"), value: "media" },
            { name: this.localize.term("redirects_url"), value: "url" }
        ];

        this.addButton = {
            alias: "add",
            label: this.localize.term("redirects_addRedirect"),
            action: this.add,
            look: "primary",
            color: "positive",
            subButtons: []
        };

        this.reloadButton = {
            alias: "reload",
            label: this.localize.term("redirects_reload"),
            action: this.reload,
            look: "outline",
            color: "default",
            subButtons: []
        };

        this.dashboard = {
            addButton: this.addButton,
            reloadButton: this.reloadButton,
            buttons: [this.addButton, this.reloadButton],
            requestUpdate: self.requestUpdate
        };

        RedirectsService.getRootNodes().then(function (res) {

            const temp = [
                { name: self.localize.term("redirects_allSites"), value: "all", selected: true },
                { name: self.localize.term("redirects_globalRedirects"), value: "00000000-0000-0000-0000-000000000000" }
            ];

            res.data.items.forEach(function (rootNode) {
                temp.push({ name: rootNode.name, value: rootNode.key });
            });

            self.rootNodes = temp;

        });

        this.updateRedirects();

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.modalManagerContext = instance;
        });

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
            this._notificationContext = instance;
        });

        window.dispatchEvent(new RedirectsDashboardLoadEvent("redirects.onDashboardLoad", {
            dashboard: self.dashboard
        }));

    }

    updateRedirects(page) {

        const self = this;

        const params = { limit: 10 };
        if (page) params.page = page;
        if (this.rootNode && this.rootNode !== "all") params.rootNodeKey = this.rootNode;
        if (this.type && this.type !== "all") params.type = this.type;
        if (this.text) params.text = this.text;

        self.loading = true;

        RedirectsService.getRedirects(params).then(function (res) {
            setTimeout(function () {
                self.redirects = res.data.items;
                self.pagination = res.data.pagination;
                self.loading = false;
                self.refreshing = false;
                self.reloadButton.state = null;
                self.requestUpdate();
            }, 200);
        });

    }

    add() {

        const self = this;

        const modalContext = this.modalManagerContext?.open(this, REDIRECTS_ADD_REDIRECT_MODAL);

        modalContext.onSubmit().then(function () {
            self.updateRedirects();
            self._notificationContext?.peek("positive", { data: { message: "Successfully added new redirect" } });
        }, function () {
            // modal closed by the user
        });

    }

    edit(redirect) {

        const self = this;

        const modalContext = this.modalManagerContext?.open(this, REDIRECTS_EDIT_REDIRECT_MODAL, {
            redirect,
            value: {
                redirect
            }
        });

        modalContext.onSubmit().then(function () {
            self.updateRedirects();
            self._notificationContext?.peek("positive", { data: { message: "Successfully updated redirect" } });
        }, function () {
            // modal closed by the user
        });

    }

    random() {
        this.meh = Math.random();
    }

    delete(redirect) {

        const self = this;

        if (!redirect?.key) return;
        umbConfirmModal(this, {
            headline: 'Delete',
            content: html`
				<div style="width:500px">
					<p>Are you sure you want to delete this redirect?</p>
					Original URL: <strong>${redirect.url}</strong><br />
					Destination URL: <strong>${redirect.destination.url}</strong>
				</div>
			`,
            color: 'danger',
            confirmLabel: 'Delete',
        }).then(function () {
            RedirectsService.deleteRedirect(redirect).then(function () {
                self._notificationContext?.peek("positive", { data: { message: "Redirect successfully deleted" } });
                self.updateRedirects();
            }, function () {
                self._notificationContext?.peek("danger", { data: { message: "Deleting redirect failed" } });
            });
        }, function () {
            // model was cancelled
        });
    }

    reload() {
        this.refreshing = true;
        this.reloadButton.state = "waiting";
        this.updateRedirects();
    }

    onKeyUp() {
        const input = this.shadowRoot.querySelector("#search");
        if (input.value == this.text) return;
        this.text = input.value;
    }

    onPageChange(e) {
        this.updateRedirects(e.target.current);
    }

    onRootNodeChange(e) {
        this.rootNode = this.shadowRoot.querySelector("#rootNode").value;
        this.updateRedirects();
    }

    onTypeChange(e) {
        this.type = this.shadowRoot.querySelector("#type").value;
        this.updateRedirects();
    }

    renderDestinationType(r) {

        let type;
        switch (r.destination.type) {
            case "url": type = this.localize.term("redirects_url"); break;
            case "content": type = this.localize.term("redirects_content"); break;
            case "media": type = this.localize.term("redirects_media"); break;
            default: type = ucfirst(r.destination.type); break;
        }

        return html`
            ${when(r.destination.name, () => html`
                ${type}:
                <small>${r.destination.name}</small>
            `, () => html`
                ${type}
            `)}
        `;

    }

    renderNoRedirects() {

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

    renderDestinationWarning(item) {
        if (item.destination.null) return html`<small class=\"warning\">${this.localize.term("redirects_deleted")}</small>`;
        if (item.destination.trashed) return html`<small class=\"warning\">${this.localize.term("redirects_trashed")}</small>`;
        if (item.destination.published === false) return html`<small class=\"warning\">${this.localize.term("redirects_unpublished")}</small>`;
        return "";
    }

    render() {
        const typeToSearch = this.localize.term("general_typeToSearch");
        return html`
            <div style="min-height: 500px;">
                <div style="display: flex;">
                    <div style="flex: 1; display: flex; gap: 10px; margin-right: 10px;">
                        <uui-select id="rootNode" label="Root node" @change="${() => this.onRootNodeChange()}" .options=${this.rootNodes}>
                            ${repeat(this.rootNodes, (item) => item.key, (item) => html`
                                <option value="${item.value}">${item.name}</option>
                            `)}
                        </uui-select>
                        <uui-select id="type" label="Type" @change="${() => this.onTypeChange()}" .options=${this.types}>
                            ${repeat(this.types, (item) => item.key, (item) => html`
                                <option value="${item.value}">${item.name}</option>
                            `)}
                        </uui-select>
                        <uui-input id="search" label="${typeToSearch}" style="flex: 1;" placeholder="${typeToSearch}" @keyup=${this.onKeyUp}></uui-input>
                    </div>
                    <div style="justify-self: right; display: flex; gap: 10px;">
                        ${repeat(this.dashboard.buttons, (button) => button.alias, (button) => html`
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
                    </div>
                </div>
                ${when(this.redirects?.length > 0, () => html`
                    <uui-box class="redirects" style="--uui-box-default-padding:0; ${(this.loading ? "opacity: 0.65;" : "")}">
                        <uui-table role="table">
                            <uui-table-head role="row">
                                <uui-table-head-cell role="columnheader">${this.localize.term("redirects_site")}</uui-table-head-cell>
                                <uui-table-head-cell role="columnheader" style="min-width: 250px;">${this.localize.term("redirects_originalUrl")}</uui-table-head-cell>
                                <uui-table-head-cell role="columnheader">${this.localize.term("redirects_type")}</uui-table-head-cell>
                                <uui-table-head-cell role="columnheader"></uui-table-head-cell>
                                <uui-table-head-cell role="columnheader" style="width: 100%;">${this.localize.term("redirects_destination")}</uui-table-head-cell>
                                <uui-table-head-cell role="columnheader"></uui-table-head-cell>
                            </uui-table-head>
                            ${repeat(this.redirects, (item) => item.key, (item) => html`
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
                                        ${this.renderDestinationWarning(item)}
                                        ${when(item.forward, () => html`
                                            <small class="forward" title="Forward query string is enabled">&nbsp;?&amp;</small>
                                        `)}
                                        <div class="displayUrl">
                                            <a href="${item.destination.displayUrl}" rel="noreferrer" target="_blank">${item.destination.displayUrl}</a>
                                        </div>
                                    </uui-table-cell>
                                    <uui-table-cell role="cell">
                                        <uui-action-bar style="justify-self: left;">
						                    <uui-button label="Edit" look="secondary" pristine="" type="button" color="default" @click="${() => this.edit(item)}">
							                    <uui-icon name="edit" aria-hidden="true"></uui-icon>
						                    </uui-button>
						                    <uui-button label="Delete" look="secondary" pristine="" type="button" color="danger" @click="${() => this.delete(item)}">
							                    <uui-icon name="delete" aria-hidden="true"></uui-icon>
						                    </uui-button>
					                    </uui-action-bar>
                                    </uui-table-cell>
                                </uui-table-row>
                            `)}
                        </uui-table>
                    </uui-box>
                    ${when(this.pagination?.pages > 1, () => html`
                        <div style="margin-top: 20px;">
                            <uui-pagination current="${this.pagination.page}" total="${this.pagination.pages}" @change="${this.onPageChange}"></uui-pagination>
                        </div>
                    `)}
                `)}
            </div>
            ${when(this.loading, () => html`<uui-loader></uui-loader>`)}
            ${when(this.redirects?.length === 0, () => this.renderNoRedirects())}
        `;
    }

    static styles = css`
        :host > div {
            padding: 20px;
        }
        uui-symbol-more {
            fill: #fff;
        }
        .warning {
            color: red;
        }
        .col-root-node {
            white-space: nowrap;
            text-overflow: ellipsis;
            overflow: hidden;
            max-width: 150px;
        }
        .col-url .warning {
            display: block;
            margin-top: -4px;
        }
        uui-box {
            margin-top: 20px;
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
        uui-loader {
            position: absolute;
            left: 50%;
            top: 50%;
            margin: -6px 0 0 -6px;
            transform: translate(-50%, -50%);
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
        .displayUrl {
            font-size: 12px;
            margin-top: -6px;
        }
    `;

}

customElements.define("redirects-dashboard", RedirectsDashboardElement);

export default RedirectsDashboardElement;