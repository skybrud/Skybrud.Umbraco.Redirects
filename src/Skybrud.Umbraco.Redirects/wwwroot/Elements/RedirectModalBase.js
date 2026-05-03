import { html, css, repeat, when, unsafeHTML } from "@umbraco-cms/backoffice/external/lit";
import { UmbModalBaseElement } from "@umbraco-cms/backoffice/modal";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from '@umbraco-cms/backoffice/notification';

import { RedirectsService } from "@skybrud-redirects/service";
import "@skybrud-redirects/elements/destination";

import { RedirectsModalLoadEvent } from "@skybrud-redirects/events";

export class RedirectModalBase extends UmbModalBaseElement {

    getPropertyGroups(redirect) {

        const groups = [
            {
                alias: "general",
                properties: [
                    {
                        alias: "site",
                        name: this.localize.term("redirectsProperties_site"),
                        type: "dropdown",
                        description: this.localize.term("redirectsProperties_siteDescription"),
                        value: redirect.rootNode?.key ?? null,
                    },
                    {
                        alias: "originalUrl",
                        name: this.localize.term("redirectsProperties_originalUrl"),
                        description: this.localize.term("redirectsProperties_originalUrlDescription"),
                        type: "textstring",
                        required: true,
                        validate: function (p) {
                            p.missing = false;
                            p.invalid = false;
                            p.error = null;
                            if (!p.value) {
                                p.missing = true;
                                p.error = this.localize.term("redirectsErrors_fieldRequired", p.name);
                                return;
                            }
                            if (p.value !== "" || p.value.indexOf("/") !== 0) {
                                p.missing = true;
                                p.error = this.localize.term("redirectsErrors_urlNotValid", p.name);
                                return;
                            }
                        }
                    },
                    {
                        alias: "destination",
                        name: this.localize.term("redirectsProperties_destination"),
                        description: this.localize.term("redirectsProperties_destinationDescription"),
                        type: "destination",
                        required: true
                    },
                    {
                        alias: "destinationCulture",
                        name: this.localize.term("redirectsProperties_destinationCulture"),
                        description: this.localize.term("redirectsProperties_destinationCultureDescription"),
                        type: "dropdown",
                        options: function () { }
                ]
            }
        ];


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
                    </div >
            <div class="property">
                <div>
                    <strong>${property("originalUrl")}<span style="color: red;">*</span></strong><br />
                    <small>${property("originalUrlDescription")}</small>
                </div>
                <div>
                    <uui-input id="originalUrl" label="${property(" originalUrl")}"></uui-input>
            </div>
                    </div >


        return groups;

    }

};