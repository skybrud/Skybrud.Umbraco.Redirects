import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";

import { UMB_MEDIA_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/media";
import { MediaService } from "@umbraco-cms/backoffice/external/backend-api";

import { RedirectsService } from "@skybrud-redirects/service";
import "@skybrud-redirects/elements/node";

async function getUrls(key) {
    const response = await MediaService.getMediaUrls({ query: { id: [key] } });
    return response.data?.find(x => x.id === key)?.urlInfos ?? [];
}

export class RedirectsMediaWorkspaceViewElement extends UmbElementMixin(LitElement) {

    #node;

    constructor() {

        super();

        const self = this;

        this.consumeContext(UMB_MEDIA_WORKSPACE_CONTEXT, (context) => {

            if (!context) return;

            // Get misc information
            const key = context.getUnique();
            const data = context.getData();

            // Get the URL(s) of the media
            getUrls(key).then(function (urls) {

                // Grab the first URL, if any. Also strip the "origin" part if the same as the current URL
                let url = urls.length > 0 ? urls[0].url : null;
                if (url?.indexOf(window.location.origin + "/") === 0) url = url.substr(window.location.origin.length);

                // Initialize the "node"
                self.#node = {
                    type: "media",
                    key: context.getUnique(),
                    name: data.variants[0].name,
                    icon: data.mediaType?.icon,
                    url: url,
                    culture: null,
                    cultures: [],
                    null: false,
                    displayUrl: url,
                    new: context.getIsNew(),
                    trashed: data.isTrashed
                };

                // Request the UI to be updated
                self.requestUpdate();

            });


        });

    }

    render() {
        return html`
            <skybrud-redirects-node .node=${this.#node}></skybrud-redirects-node>
        `;
    }

    static styles = css`

        :host {
            display: block;
            height: 100%;
        }

    `;

};

customElements.define("redirects-media-workspace-view", RedirectsMediaWorkspaceViewElement);

export default RedirectsMediaWorkspaceViewElement;