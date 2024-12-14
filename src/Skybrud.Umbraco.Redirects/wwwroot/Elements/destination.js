import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, when } from "@umbraco-cms/backoffice/external/lit";
import "@umbraco-cms/backoffice/components";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { UMB_LINK_PICKER_MODAL } from "@umbraco-cms/backoffice/multi-url-picker";

import { RedirectsService } from "@skybrud-redirects/service";

export class RedirectsDestinationElement extends UmbElementMixin(LitElement) {

    get value() {
        return this._value;
    }

    set value(v) {
        this._value = v;
        this.requestUpdate();
    }

    constructor() {

        super();

        this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (instance) => {
            this.modalManagerContext = instance;
        });

        this.consumeContext(UMB_NOTIFICATION_CONTEXT, (instance) => {
            this.notificationContext = instance;
        });

    }


    connectedCallback() {

        super.connectedCallback();

        const value = this.innerText;

        try {
            if (value) {
                this.value = JSON.parse(value);
                this.requestUpdate();
            }
        } catch (ex) {
            console.error(ex);
        }

    }

    add() {

        const self = this;

        const modalContext = this.modalManagerContext?.open(this, UMB_LINK_PICKER_MODAL);

        modalContext.onSubmit().then(function (value) {


            if (!value.link) {
                alert("No link");
                return;
            }

            if (!value.link.url) {
                alert("No link URL");
                return;
            }

            switch (value.link.type) {

                case "document":
                    RedirectsService.getContent(value.link.unique).then(function (res) {
                        self.value = {
                            type: "content",
                            key: res.data.id,
                            name: res.data.variants[0].name,
                            url: res.data.urls[0].url,
                            cultures: res.data.variants.filter(x => x.culture).map(x => x.culture)
                        };
                    });
                    break;

                case "media":
                    RedirectsService.getMedia(value.link.unique).then(function (res) {
                        self.value = {
                            type: "media",
                            key: res.data.id,
                            name: res.data.variants[0].name,
                            url: res.data.urls[0].url
                        };
                    });
                    break;

                case "external":
                    self.value = {
                        type: "external",
                        url: value.url
                    };
                    break;

            }

        }, function () {

            // Model closed by the user

        });

    }

    reset() {
        this.value = null;
    }

    edit() {
        this.add();
    }

    render() {
        return html`
            <div>
                ${when(!this.value, () => html`
                    <uui-button class="add-btn" look="placeholder" color="default" label="Add" @click=${this.add}>Add</uui-button>
                `)}
                ${when(this.value, () => html`
                    <uui-ref-node name="${this.value.name}" detail="${this.value.url}" selectable="false" selectOnly="true">
                      <uui-action-bar slot="actions">
                        <uui-button @click="${this.edit}" label="${this.localize.term("general_edit")}"><uui-icon name="edit" aria-hidden="true"></uui-icon></uui-button>
                        <uui-button @click="${this.reset}" label="${this.localize.term("general_delete")}" color="danger"><uui-icon name="delete" aria-hidden="true"></uui-icon></uui-button>
                      </uui-action-bar>
                    </uui-ref-node>
                `)}
            </div>
        `;

    }

    static styles = css`
        pre {
            display: block;
            font-family: monospace;
            font-size: 11px;
            line-height: 15px;
            padding: 9.5px;
            background-color: #f6f4f4;
            border: 1px solid #d8d7d9;
            border-radius: 3px;
        }
        uui-ref-node {
            border: 1px solid var(--uui-color-border);
        }
        div > uui-button {
            width: 100%;
        }
    `;

};

customElements.define("redirects-destination", RedirectsDestinationElement);

export default RedirectsDestinationElement;