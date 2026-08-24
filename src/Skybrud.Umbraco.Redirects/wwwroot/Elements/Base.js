import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { LitElement, html, css, repeat, when } from "@umbraco-cms/backoffice/external/lit";

import { DocumentService } from "@umbraco-cms/backoffice/external/backend-api";
import { MediaService } from "@umbraco-cms/backoffice/external/backend-api";

import { UMB_PROPERTY_DATASET_CONTEXT } from "@umbraco-cms/backoffice/property";
import { UMB_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/workspace";

import { RedirectsService } from "@skybrud-redirects/service";

async function getDocumentUrls(key) {
    const response = await DocumentService.getDocumentUrls({ query: { id: [key] } });
    return response.data?.find(x => x.id === key)?.urlInfos ?? [];
}

async function getMediaUrls(key) {
    const response = await MediaService.getMediaUrls({ query: { id: [key] } });
    return response.data?.find(x => x.id === key)?.urlInfos ?? [];
}

export class RedirectsElementBase extends UmbElementMixin(LitElement) {

    constructor() {
        super();
    }

    getCurrentNode(callback) {

        const self = this;

        self.consumeContext(UMB_WORKSPACE_CONTEXT, (context) => {

            if (!context) return;

            // We need the property dataset context order to determine the current variation/culture
            this.consumeContext(UMB_PROPERTY_DATASET_CONTEXT, (datasetContext) => {

                if (!datasetContext) return;

                // Get current variant, then the culture of said variant
                const variantId = datasetContext.getVariantId();
                const culture = variantId.culture;

                // Get misc information
                const key = context.getUnique();
                const type = context.getEntityType();
                const data = context.getData();

                switch (type) {

                    case "document":

                        getDocumentUrls(key).then(function (urls) {

                            // Get the variant specific name and URL
                            const name = (data.variants.find(x => x.culture == culture) ?? data.variants[0]).name;
                            let url = (urls.find(x => x.culture == culture) ?? urls[0])?.url;
                            if (url && url.indexOf(window.location.origin + "/") === 0) url = url.substr(window.location.origin.length);

                            const cultures = [];

                            RedirectsService.getCultures(key).then(function (res) {
                                callback({
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
                                });
                            });

                        });

                        break;

                    case "media":

                        getMediaUrls(key).then(function (urls) {

                            // Grab the first URL, if any. Also strip the "origin" part if the same as the current URL
                            let url = urls.length > 0 ? urls[0].url : null;
                            if (url?.indexOf(window.location.origin + "/") === 0) url = url.substr(window.location.origin.length);

                            // Initialize the "node"
                            const node = {
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

                            callback(node);

                        });

                        break;

                    default:
                        console.error(`Unsupported entity type '${type}'.`);
                        break;

                }

            });

        });

    }

}

export default RedirectsElementBase;