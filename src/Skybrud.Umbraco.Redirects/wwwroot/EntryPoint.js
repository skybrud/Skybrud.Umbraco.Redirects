import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { RedirectsAuth } from "@skybrud-redirects/auth";
import { RedirectsPackage } from "@skybrud-redirects/package";
import { RedirectsService } from "@skybrud-redirects/service";

import "@skybrud-redirects/elements/from-now";

export const onInit = (_host, extensionRegistry) => {

    _host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {

        const config = authContext.getOpenApiConfiguration();
        RedirectsAuth.TOKEN = config.token;

        RedirectsService.getServerVariables().then(function (res) {

            RedirectsPackage.serverVariables = res.data;

            extensionRegistry.register({
                type: "dashboard",
                name: "Redirects",
                alias: "Skybrud.Umbraco.Redirects.Dashboard",
                elementName: "redirects-dashboard",
                js: () => import("./Elements/Dashboard.js?v=" + RedirectsPackage.cacheBuster),
                weight: -10,
                meta: {
                    label: "Redirects",
                    pathname: "redirects"
                },
                conditions: [
                    {
                        alias: "Umb.Condition.SectionAlias",
                        match: "Umb.Section.Content"
                    }
                ]
            });

            extensionRegistry.register({
                "type": "localization",
                "alias": "Skybrud.Umbraco.Redirects.En",
                "name": "English",
                "js": () => import("./Localization/en-US.js?v=" + RedirectsPackage.cacheBuster),
                "meta": {
                    "culture": "en"
                }
            });

            extensionRegistry.register({
                "type": "localization",
                "alias": "Skybrud.Umbraco.Redirects.EnUS",
                "name": "English (United States)",
                "js": () => import("./Localization/en-US.js?v=" + RedirectsPackage.cacheBuster),
                "meta": {
                    "culture": "en-US"
                }
            });

            extensionRegistry.register({
                "type": "localization",
                "alias": "Skybrud.Umbraco.Redirects.DaDk",
                "name": "Danish (Denmark)",
                "js": () => import("./Localization/da-DK.js?v=" + RedirectsPackage.cacheBuster),
                "meta": {
                    "culture": "da-DK"
                }
            });

            extensionRegistry.register({
                "type": "modal",
                "alias": "Skybrud.Umbraco.Redirects.AddRedirectModal",
                "name": "Add Redirect Modal",
                "element": "/App_Plugins/Skybrud.Umbraco.Redirects/Modals/add-redirect.element.js?v=" + RedirectsPackage.cacheBuster,
            });

            extensionRegistry.register({
                "type": "modal",
                "alias": "Skybrud.Umbraco.Redirects.EditRedirectModal",
                "name": "Edit Redirect Modal",
                "element": "/App_Plugins/Skybrud.Umbraco.Redirects/Modals/edit-redirect.element.js?v=" + RedirectsPackage.cacheBuster,
            });

        });

    });

};