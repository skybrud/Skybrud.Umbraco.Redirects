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
import { UMB_MEDIA_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/media";

function ucfirst(value) {
    return String(value).charAt(0).toUpperCase() + String(value).slice(1);
}

export class RedirectsMediaWorkspaceViewElement extends UmbElementMixin(LitElement) {

    #workspace;

    constructor() {

        super();

        const self = this;

        this.consumeContext(UMB_MEDIA_WORKSPACE_CONTEXT, (context) => {

            this.#workspace = context;

            if (!context) return;

            console.log(context);
            console.log(context.getData());

        });

    }

    render() {
        return html`<div>Hello there!</div>`;
    }

};

customElements.define("redirects-media-workspace-view", RedirectsMediaWorkspaceViewElement);

export default RedirectsMediaWorkspaceViewElement;