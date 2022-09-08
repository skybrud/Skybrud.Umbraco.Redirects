angular.module("umbraco").directive("skybrudRedirectsNode", function (localizationService, notificationsService, overlayService, $http, $q, $timeout, skybrudRedirectsService) {

    // Get the cache buster value
    const cacheBuster = Umbraco.Sys.ServerVariables.skybrud.redirects.cacheBuster;

    // Get the base URL for the API controller
    const baseUrl = Umbraco.Sys.ServerVariables.skybrud.redirects.baseUrl;

    return {
        restrict: "E",
        templateUrl: `/App_Plugins/Skybrud.Umbraco.Redirects/Views/Directives/Node.html?v=${cacheBuster}`,
        scope: {
            node: "=",
            config: "=?",
            title: "=",
            description: "="
        },
        link: function(scope) {

            scope.loading = false;
            scope.redirects = [];

            // If we"re neither in the content or media section, we stop further execution (eg. property editor preview)
            if (!scope.node || scope.node.type !== "content" && scope.node.type !== "media") return;
            scope.node.create = scope.node.mode === "create";

            // Make sure we have a configuration
            if (!scope.config) scope.config = {};
            if (scope.config.hideTitle === undefined) scope.config.hideTitle = true;

            scope.rootNodes = [
                { name: "All sites", value: "" }
            ];

            localizationService.localize("redirects_allSites").then(function (value) {
                scope.rootNodes[0].name = value;
            });

            skybrudRedirectsService.getRootNodes().then(function (r) {
                r.data.items.forEach(function (rootNode) {
                    scope.rootNodes.push(rootNode);
                });
            });


            if (!scope.config) scope.config = {};
            if (!scope.config.hideRootNodeOption) scope.config.hideRootNodeOption = false;

            scope.addRedirect = function () {
                skybrudRedirectsService.addRedirect({
                    destination: scope.node,
                    hideRootNodeOption: scope.config.hideRootNodeOption,
                    rootNodes: scope.rootNodes,
                    callback: function () {
                        scope.updateList();
                    }
                });
            };

            scope.editRedirect = function (redirect) {
                skybrudRedirectsService.editRedirect(redirect, {
                    hideRootNodeOption: scope.config.hideRootNodeOption,
                    rootNodes: scope.rootNodes,
                    callback: function () {
                        scope.updateList();
                    }
                });
            };

            scope.deleteRedirect = function (redirect) {
                skybrudRedirectsService.requestDeleteRedirect({
                    redirect: redirect,
                    submit: function () {
                        scope.updateList();
                        overlayService.close();
                    }
                });
            };

            scope.updateList = function () {

                scope.loading = true;

                // Make the call to the redirects API
                const http = $http({
                    method: "GET",
                    url: `${baseUrl}GetRedirectsForNode`,
                    params: {
                        type: scope.node.type,
                        key: scope.node.key
                    }
                });

                // Show the loader for at least 200 ms
                const timer = $timeout(function () { }, 200);

                // Wait for both the AJAX call and the timeout
                $q.all([http, timer]).then(function (array) {
                    scope.content = array[0].data.content;
                    scope.redirects = array[0].data.redirects;
                    scope.loading = false;
                }, function () {
                    notificationsService.error("Unable to load redirects", "The list of redirects could not be loaded.");
                    scope.loading = false;
                });

            };

            if (scope.node.mode === "list") scope.updateList();

        }
    };

});