import { UMB_AUTH_CONTEXT } from "@umbraco-cms/backoffice/auth";

import { RedirectsAuth } from "@skybrud-redirects/auth";
import { RedirectsPackage } from "@skybrud-redirects/package";
import { RedirectsService } from "@skybrud-redirects/service";
import { RedirectsDashboardCondition } from "@skybrud-redirects/conditions/dashboard";
import { RedirectsDocumentCondition } from "@skybrud-redirects/conditions/document";
import { RedirectsMediaCondition } from "@skybrud-redirects/conditions/media";

import "@skybrud-redirects/elements/from-now";


export const onInit = (_host, extensionRegistry) => {

    _host.consumeContext(UMB_AUTH_CONTEXT, (authContext) => {

        const config = authContext.getOpenApiConfiguration();
        RedirectsAuth.TOKEN = config.token;

        RedirectsService.getServerVariables().then(function (res) {

            RedirectsPackage.serverVariables = res.data;

            if (RedirectsPackage.settings?.dashboard?.enabled) {
                extensionRegistry.register({
                    type: "dashboard",
                    name: "Redirects",
                    alias: "Skybrud.Umbraco.Redirects.Dashboard",
                    elementName: "redirects-dashboard",
                    js: () => import("./Elements/Dashboard.js?v=" + RedirectsPackage.cacheBuster),
                    weight: RedirectsPackage.settings?.dashboard?.weight ?? -10,
                    meta: {
                        label: "Redirects",
                        pathname: "redirects"
                    },
                    conditions: [
                        {
                            alias: "Umb.Condition.SectionAlias",
                            match: "Umb.Section.Content"
                        },
                        {
                            alias: "Skybrud.Umbraco.Redirects.Condition.Dashboard"
                        }
                    ]
                });
            }

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
                "name": "English",
                "js": () => import("./Localization/en-US.js?v=" + RedirectsPackage.cacheBuster),
                "meta": {
                    "culture": "en"
                }
            });

            extensionRegistry.register({
                "type": "localization",
                "alias": "Skybrud.Umbraco.Redirects.DaDk",
                "name": "Danish",
                "js": () => import("./Localization/da-DK.js?v=" + RedirectsPackage.cacheBuster),
                "meta": {
                    "culture": "da"
                }
            });

            extensionRegistry.register({
                "type": "localization",
                "alias": "Skybrud.Umbraco.Redirects.SvSe",
                "name": "Swedish",
                "js": () => import("./Localization/sv-SE.js?v=" + RedirectsPackage.cacheBuster),
                "meta": {
                    "culture": "sv"
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

            extensionRegistry.register({
                type: "condition",
                name: "Skybrud Redirects: Document Condition",
                alias: "Skybrud.Umbraco.Redirects.Condition.Document",
                api: RedirectsDocumentCondition
            });

            extensionRegistry.register({
                type: "condition",
                name: "Skybrud Redirects: Media Condition",
                alias: "Skybrud.Umbraco.Redirects.Condition.Media",
                api: RedirectsMediaCondition
            });

            extensionRegistry.register({
                type: "condition",
                name: "Skybrud Redirects: Dashboard Condition",
                alias: "Skybrud.Umbraco.Redirects.Condition.Dashboard",
                api: RedirectsDashboardCondition
            });

            extensionRegistry.register({
                type: "propertyEditorUi",
                alias: "Skybrud.Umbraco.Redirects",
                name: 'Skybrud Redirects: Inbound Redirects Property Editor UI',
                element: () => import(`./Elements/InboundRedirects.js?v=${RedirectsPackage.cacheBuster}`),
                meta: {
                    label: "Inbound Redirects",
                    icon: "icon-arrow-right",
                    group: "Skybrud",
                    propertyEditorSchemaAlias: "Skybrud.Umbraco.Redirects"
                },
            });

            extensionRegistry.register({
                type: "propertyEditorUi",
                alias: "Skybrud.Umbraco.Redirects.Outbound",
                name: 'Skybrud Redirects: Outbound Redirect Property Editor UI',
                element: () => import(`./Elements/OutboundRedirect.js?v=${RedirectsPackage.cacheBuster}`),
                meta: {
                    label: "Outbound Redirect",
                    icon: "icon-arrow-right",
                    group: "Skybrud",
                    propertyEditorSchemaAlias: "Skybrud.Umbraco.Redirects.OutboundRedirect"
                },
            });

            if (RedirectsPackage.settings?.workspaceViews?.document?.enabled) {
                extensionRegistry.register({
                    type: "workspaceView",
                    name: "Skybrud Redirects: Document Workspace View",
                    alias: "Skybrud.Umbraco.Redirects.DocumentWorkspaceView",
                    element: "/App_Plugins/Skybrud.Umbraco.Redirects/WorkspaceViews/Document.js?v=" + RedirectsPackage.cacheBuster,
                    weight: RedirectsPackage.settings?.workspaceViews?.document?.weight ?? -100,
                    meta: {
                        label: "Redirects",
                        pathname: "redirects",
                        icon: "icon-arrow-right"
                    },
                    conditions: [
                        {
                            alias: "Umb.Condition.WorkspaceAlias",
                            match: "Umb.Workspace.Document"
                        },
                        {
                            alias: "Skybrud.Umbraco.Redirects.Condition.Document"
                        }
                    ]
                });
            }

            if (RedirectsPackage.settings?.workspaceViews?.media?.enabled) {
                extensionRegistry.register({
                    type: "workspaceView",
                    name: "Skybrud Redirects: Media Workspace View",
                    alias: "Skybrud.Umbraco.Redirects.MediaWorkspaceView",
                    element: "/App_Plugins/Skybrud.Umbraco.Redirects/WorkspaceViews/Media.js?v=" + RedirectsPackage.cacheBuster,
                    weight: RedirectsPackage.settings?.workspaceViews?.media?.weight ?? -100,
                    meta: {
                        label: "Redirects",
                        pathname: "redirects",
                        icon: "icon-arrow-right"
                    },
                    conditions: [
                        {
                            alias: "Umb.Condition.WorkspaceAlias",
                            match: "Umb.Workspace.Media"
                        },
                        {
                            alias: "Skybrud.Umbraco.Redirects.Condition.Media"
                        }
                    ]
                });
            }

        });

    });

};