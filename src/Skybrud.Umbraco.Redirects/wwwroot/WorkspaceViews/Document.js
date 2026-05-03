import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";

import '@umbraco-cms/backoffice/components';

import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

import { RedirectsPackage } from "@skybrud-redirects/package";
import { RedirectsService } from "@skybrud-redirects/service";
import { REDIRECTS_ADD_REDIRECT_MODAL } from "@skybrud-redirects/modals/add";
import { REDIRECTS_EDIT_REDIRECT_MODAL } from "@skybrud-redirects/modals/edit";

import { RedirectsDashboardLoadEvent } from "@skybrud-redirects/events";

import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";

function ucfirst(value) {
    return String(value).charAt(0).toUpperCase() + String(value).slice(1);
}

export class RedirectsWorkspaceViewElement extends UmbElementMixin(LitElement) {

    #workspace;

    constructor() {

        super();

        const self = this;

        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (context) => {

            this.#workspace = context;

            if (!context) return;

            self.entity = {
                unique: context.getUnique(),
                isNew: context.getIsNew(),
                //isTrashed: context.isTrashed(),
                data: context.getData(),
            };

            self.entity.isTrashed = self.entity.data.isTrashed;
            self.entity.isPublished = context.getVariants()?.some(v => v.state === "Published");

            console.log(self.data);

            self.requestUpdate();

        });

    }

    render() {

        if (this.entity.isNew) {
            return html`
                <div>You are in the process of creating this page, meaning you cannot yet add redirects to it. Save the page to continue, then you'll be able to add redirects.</div>
            `;
        }

        if (this.entity.isTrashed) {
            return html`
                <div>This page is current trashed. Any redirect pointing to this page will not work.</div>
            `;
        }

        if (!this.entity.isPublished) {
            return html`
                <div>This page is currently not published, meaning redirects pointing to it may not work.</div>
            `;
        }

        return html`<div>
            --<pre>${JSON.stringify(this.entity, null, 2)}</pre>--
        </div>`;

    }

};

customElements.define("redirects-workspace-view", RedirectsWorkspaceViewElement);

export default RedirectsWorkspaceViewElement;