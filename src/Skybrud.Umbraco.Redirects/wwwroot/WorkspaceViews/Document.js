import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";

import { UMB_PROPERTY_DATASET_CONTEXT } from "@umbraco-cms/backoffice/property";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";
import { DocumentService } from "@umbraco-cms/backoffice/external/backend-api";

import { RedirectsService } from "@skybrud-redirects/service";
import "@skybrud-redirects/elements/node";

async function getUrls(key) {
    const response = await DocumentService.getDocumentUrls({ query: { id: [key] } });
    return response.data?.find(x => x.id === key)?.urlInfos ?? [];
}

export class RedirectsWorkspaceViewElement extends UmbElementMixin(LitElement) {

    #node;

    constructor() {

        super();

        const self = this;

        // We need the property dataset context order to determine the current variation/culture
        this.consumeContext(UMB_PROPERTY_DATASET_CONTEXT, (datasetContext) => {

            if (!datasetContext) return;

            // Get current variant, then the culture of said variant
            const variantId = datasetContext.getVariantId();
            const culture = variantId.culture;

            // We need the workspace context in order to get various information about the current page
            self.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (context) => {

                if (!context) return;

                // Get misc information
                const key = context.getUnique();
                const data = context.getData();

                // The workspace context doesn't expose the page's URLs, so we need to fetch those separately
                getUrls(key).then(function (urls) {

                    // Get the variant specific name and URL
                    const name = (data.variants.find(x => x.culture == culture) ?? data.variants[0]).name;
                    let url = (urls.find(x => x.culture == culture) ?? urls[0]).url;
                    if (url.indexOf(window.location.origin + "/") === 0) url = url.substr(window.location.origin.length);

                    const cultures = [];

                    RedirectsService.getCultures(key).then(function (res) {
                        self.entity = {
                            type: "content",
                            key: key,
                            name: name,
                            icon: data.documentType.icon,
                            url: url,
                            culture: culture,
                            cultures: data.variants.filter(x => x.culture).map(x => x.culture),
                            null: false,
                            new: context.getIsNew(),
                            trashed: data.isTrashed,
                            published: context.getVariants()?.some(v => v.state === "Published"),
                            displayUrl: url,
                            __cultures: res.data
                        };
                        self.requestUpdate();
                    });

                });

            });

        });

    }

    render() {
        return html`
            <skybrud-redirects-node .node=${this.entity}></skybrud-redirects-node>
        `;
    }

    static styles = css`

        :host {
            display: block;
            height: 100%;
        }

    `;

};

customElements.define("redirects-workspace-view", RedirectsWorkspaceViewElement);

export default RedirectsWorkspaceViewElement;