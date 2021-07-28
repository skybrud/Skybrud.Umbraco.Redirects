angular.module("umbraco").controller("SkybrudUmbracoRedirects.PropertyEditor.Controller", function ($scope, $routeParams, $http, $q, $timeout, localizationService, notificationsService, overlayService, skybrudRedirectsService, editorState) {

    // Get the base URL for the API controller
    const baseUrl = Umbraco.Sys.ServerVariables.skybrud.redirects.baseUrl;

    $scope.route = $routeParams;
    $scope.redirects = [];

    $scope.mode = $routeParams.create ? "create" : "list";
    $scope.type = $scope.route.section;

    $scope.loading = false;

    $scope.showTitle = $scope.model.config !== "1";
    
    // If we"re neither in the content or media section, we stop further execution (eg. property editor preview)
    if ($scope.type !== "content" && $scope.type !== "media") return;

    // Get the current editor state (the content or media being edited)
    var state = editorState.getCurrent();



    $scope.rootNodes = [
        { name: "All sites", value: "" }
    ];

    localizationService.localize("redirects_allSites").then(function (value) {
        $scope.rootNodes[0].name = value;
    });

    skybrudRedirectsService.getRootNodes().then(function (r) {
        r.data.items.forEach(function (rootNode) {
            $scope.rootNodes.push(rootNode);
        });
    });


    $scope.addRedirect = function () {

        // Initialize a new object representing the destination
        var destination = {
            id: state.id,
            key: state.key,
            name: state.variants ? state.variants[0].name : state.name,
            url: state.mediaLink ? state.mediaLink : state.urls[0].text,
            type: state.udi.split("/")[2] === "media" ? "media" : "content"
        };

        skybrudRedirectsService.addRedirect({
            destination: destination,
            hideRootNodeOption: $scope.model.config.hideRootNodeOption,
            rootNodes: $scope.rootNodes,
            callback: function () {
                $scope.updateList();
            }
        });

    };

    $scope.editRedirect = function (redirect) {
        skybrudRedirectsService.editRedirect(redirect, {
            hideRootNodeOption: $scope.model.config.hideRootNodeOption,
            rootNodes: $scope.rootNodes,
            callback: function () {
                $scope.updateList();
            }
        });
    };

    $scope.deleteRedirect = function (redirect) {

        const url = redirect.url + (redirect.queryString ? `?${redirect.queryString}` : "");

        // TODO: Localize overlay labels

        const overlay = {
            title: "Confirm delete",
            content: `Are you sure you want to delete the redirect at "${url}" ?`,
            submit: function () {

                // Update the button state in the UI
                overlay.submitButtonState = "busy";

                // Delete the redirect
                skybrudRedirectsService.deleteRedirect(redirect, function () {
                    overlayService.close();
                    $scope.updateList();
                }, function () {
                    overlay.submitButtonState = "error";
                });

            },
            close: function () {
                overlayService.close();
            }
        };

        // Open the overlay
        overlayService.confirmDelete(overlay);

    };

    $scope.updateList = function () {

        $scope.loading = true;

        // Make the call to the redirects API
        const http = $http({
            method: "GET",
            url: `${baseUrl}GetRedirectsFor${$scope.type}`,
            params: {
                contentId: $routeParams.id
            }
        });

        // Show the loader for at least 200 ms
        const timer = $timeout(function () { }, 200);

        // Wait for both the AJAX call and the timeout
        $q.all([http, timer]).then(function (array) {
            $scope.content = array[0].data.data.content;
            $scope.redirects = array[0].data.data.redirects;
            $scope.loading = false;
        }, function () {
            notificationsService.error("Unable to load redirects", "The list of redirects could not be loaded.");
            $scope.loading = false;
        });

    };

    if ($scope.mode === "list") {
        $scope.updateList();
    }

});