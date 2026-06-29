import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, when } from "@umbraco-cms/backoffice/external/lit";
import "@umbraco-cms/backoffice/components";
import { UMB_MODAL_MANAGER_CONTEXT } from "@umbraco-cms/backoffice/modal";
import { UMB_NOTIFICATION_CONTEXT } from "@umbraco-cms/backoffice/notification";
import { UMB_LINK_PICKER_MODAL } from "@umbraco-cms/backoffice/multi-url-picker";

import { RedirectsService } from "@skybrud-redirects/service";

import { FAKE_UMB_PROPERTY_DATASET_CONTEXT, FakeUmbPropertyDatasetContext } from "/App_Plugins/Skybrud.Umbraco.Redirects/UmbPropertyDatasetContext.js";

import { extractUmbColorVariable } from "@umbraco-cms/backoffice/resources";

function parseMediaUrl(url) {

    // The link picker modal (14.3.0) returns an absolute URL for media, where we just want the relative URL
    if (url.indexOf(window.location.origin + "/") === 0) url = url.substr(window.location.origin.length);

    return url;

}

// Our destination/link object is slightly different from Umbraco's, so we need to convert it into
// something Umbraco can understand
function toUmbracoLink(value) {

    if (!value) return null;

    const link = {
        name: value.name,
        type: value.type === "content" ? "document" : (value.type === "url" ? "external" : value.type),
        url: value.url
    };

    if (value.key) link.unique = value.key;
    if (value.icon) link.icon = value.icon;
    if (value.query) link.queryString = value.query[0] === "?" ? value.query : "?" + value.query;
    if (value.fragment) link.queryString = (link.queryString ?? "") + value.fragment;

    return link;

}

function addQueryAndFragment(target, source) {

    // If a query string value has been specified, we need to separate the actual query string and
    // the fragment, as the field in the UI may be used for both
    if (source?.queryString) {
        if (source.queryString === "#") return;
        const pos = source.queryString.indexOf("#");
        if (pos >= 0) {
            target.query = source.queryString.substr(0, pos);
            target.fragment = source.queryString.substr(pos);
        } else {
            target.query = source.queryString;
            target.fragment = null;
        }
        if (target.query && target.query[0] === "?") target.query = target.query.substr(1);
        return;
    }

    // If the query string value wasn't specified, we should check the specified URL for a
    // fragment.If found, we strip it from the URL and add a "fragment" property instead
    const pos2 = target.url.indexOf("#");
    if (pos2 >= 0) {
        target.fragment = target.url.substr(pos2);
        target.url = target.url.substr(0, pos2);
    }

    // And then also if the URL contains a query string part, we strip that in a similar way
    const pos3 = target.url.indexOf("?");
    if (pos3 >= 0) {
        target.query = target.url.substr(pos3 + 1);
        target.url = target.url.substr(0, pos3);
    }

}

function fromContent(value, content) {

    const link = {
        type: "content",
        key: content.id,
        name: content.variants[0].name,
        icon: content.documentType.icon,
        url: content.url,
        cultures: content.variants.filter(x => x.culture).map(x => x.culture),
        null: false,
        trashed: content.isTrashed,
        published: true
    };

    // If the selected content only has a single variant, and that variant is a draft, we know that
    // the content currently isn't published
    // TODO: support variants
    if (content.variants.length && content.variants[0].state == "Draft") {
        link.published = false;
    }

    addQueryAndFragment(link, value);

    // Make sure that we set the display URL with proper path, query string and fragment
    link.displayUrl = link.url + (link.query ? "?" + link.query : "") + (link.fragment ?? "");

    return link;

}

function fromMedia(value, media) {

    const icon = media.mediaType.icon.split(" ");

    const link = {
        type: "media",
        key: media.id,
        name: media.variants[0].name,
        icon: icon[0],
        color: icon.length > 1 ? icon.length[1] : null,
        url: parseMediaUrl(media.url)
    };

    addQueryAndFragment(link, value);

    // Make sure that we set the display URL with proper path, query string and fragment
    link.displayUrl = link.url + (link.query ? "?" + link.query : "") + (link.fragment ?? "");

    return link;

}

function fromExternal(value) {

    const link = {
        type: "url",
        key: "00000000-0000-0000-0000-000000000000",
        url: value.url,
        name: value.name,
        icon: "icon-link",
        color: null
    };

    addQueryAndFragment(link, value);

    // Make sure that we set the display URL with proper path, query string and fragment
    link.displayUrl = link.url + (link.query ? "?" + link.query : "") + (link.fragment ?? "");

    return link;

}

function parseIcon(value) {
    if (!value) return null;
    const array = value.split(" ");
    const color = array.length > 1 ? extractUmbColorVariable(array[1].replace("color-", "")) : null;
    return { name: array[0], color: color ? `color:var(${color})` : null };
}

function updateItem(item) {

    if (!item) return null;

    const icon = parseIcon(item.icon);

    if (icon) {
        item.iconName = icon.name;
        item.iconColor = icon.color;
    }

    return item;

}

export class RedirectsDestinationElement extends UmbElementMixin(LitElement) {

    get value() {
        return this._value;
    }

    set value(v) {
        updateItem(v);
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

        this.provideContext(FAKE_UMB_PROPERTY_DATASET_CONTEXT, new FakeUmbPropertyDatasetContext(this, FAKE_UMB_PROPERTY_DATASET_CONTEXT));

    }

    connectedCallback() {

        super.connectedCallback();

        const value = this.innerText;

        try {
            if (value) {
                this.value = updateItem(JSON.parse(value));
                this.requestUpdate();
            }
        } catch (ex) {
            console.error(ex);
        }

    }

    edit() {

        const self = this;

        const modalContext = this.modalManagerContext?.open(this, UMB_LINK_PICKER_MODAL, {
            data: {
                config: {
                    hideTarget: true
                },
                index: null,
            },
            modal: { size: 'medium' },
            value: {
                link: toUmbracoLink(self.value) ?? {},
            },
        });

        modalContext.onSubmit().then(function (value) {

            if (!value.link) {
                alert("No link");
                return;
            }

            switch (value.link.type) {

                case "document":
                    RedirectsService.getContent(value.link.unique).then(function (res) {
                        res.data.url = res.data.urls.find(x => x);
                        if (!res.data.url) {
                            alert("No link URL");
                            return;
                        }
                        self.value = updateItem(fromContent(value.link, res.data));
                        self.dispatchEvent(new CustomEvent("change", { detail: { value: self.value }, bubbles: true, composed: true }));
                    });
                    break;

                case "media":
                    if (!value.link.url) {
                        alert("No link URL");
                        return;
                    }
                    RedirectsService.getMedia(value.link.unique).then(function (res) {
                        self.value = updateItem(fromMedia(value.link, res.data));
                        self.dispatchEvent(new CustomEvent("change", { detail: { value: self.value }, bubbles: true, composed: true }));
                    });
                    break;

                case "external":
                    if (!value.link.url) {
                        alert("No link URL");
                        return;
                    }
                    self.value = updateItem(fromExternal(value.link));
                    self.dispatchEvent(new CustomEvent("change", { detail: { value: self.value }, bubbles: true, composed: true }));
                    break;

            }

        }, function () {

            // Modal closed by the user

        });

    }

    reset() {
        this.value = null;
        this.dispatchEvent(new CustomEvent("change", { detail: { value: this.value }, bubbles: true, composed: true }));
    }

    setName(name) {
        if (!this.value || !name) return;
        this.value.name = name;
        this.requestUpdate();
    }

    setUrl(url) {
        if (!this.value) return;

        this.value.url = url;
        this.value.displayUrl = url + (this.value.query ? "?" + this.value.query : "") + (this.value.fragment ? this.value.fragment : "");

        this.requestUpdate();
    }

    render() {
        return html`
            <div>
                ${when(!this.value, () => html`
                    <uui-button class="add-btn" look="placeholder" color="default" label="${this.localize.term("general_add")}" @click=${this.edit}>
                        ${this.localize.term("general_add")}
                    </uui-button>
                `)}
                ${when(this.value, () => html`
                    ${when(this.value.published === false, () => html`
                        <div style="color: red;">${this.localize.term("redirects_contentNotPublished")}</div>
                    `)}
                    <uui-ref-node name="${this.value.name}" detail="${this.value.displayUrl}" selectable="false" selectOnly="true">
                      <uui-icon slot="icon" name="${this.value.iconName}" style="${this.value.iconColor}"></uui-icon>
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