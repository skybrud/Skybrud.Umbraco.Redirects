angular.module("umbraco").controller("SkybrudUmbracoRedirects.Dashboard.Controller", function ($scope, $http, $q, $timeout, overlayService, notificationsService, localizationService, skybrudRedirectsService, eventsService) {

    // Get the base URL for the API controller
    const baseUrl = Umbraco.Sys.ServerVariables.skybrud.redirects.baseUrl;

    const vm = this;

    vm.redirects = [];
    vm.mode = "list";

    vm.rootNodes = [
        { name: "All sites", key: "" },
        { name: "Global redirects", key: "00000000-0000-0000-0000-000000000000" }
    ];

    localizationService.localize("redirects_allSites").then(function (value) {
        vm.rootNodes[0].name = value;
    });

    localizationService.localize("redirects_globalRedirects").then(function (value) {
        vm.rootNodes[1].name = value;
    });

    vm.types = [
        { name: "All types", value: "all" },
        { name: "Content", value: "content" },
        { name: "Media", value: "media" },
        { name: "URL", value: "url" }
    ];

    vm.types.forEach(function (type) {
        localizationService.localize(`redirects_media_${type.value}`).then(function (value) {
            type.name = value;
        });
    });

    vm.filters = {
        rootNode: vm.rootNodes[0],
        type: vm.types[0],
        text: ""
    };

    vm.activeFilters = 0;

    vm.loading = false;

    // Opens a dialog for adding a new redirect. When a callback received, the list is updated.
    // Since we don't have any sorting, we can assume the added redirect will be shown last
    vm.addRedirect = function () {
        skybrudRedirectsService.addRedirect({
            rootNodes: vm.rootNodes,
            callback: function () {
                vm.updateList(1);
            }
        });
    };

    // Opens a dialog for adding a new redirect. When a callback received, the list is updated.
    vm.editRedirect = function (redirect) {
        skybrudRedirectsService.editRedirect(redirect, {
            rootNodes: vm.rootNodes,
            callback: function () {
                vm.updateList();
            }
        });
    };

    vm.deleteRedirect = function (redirect) {
        skybrudRedirectsService.requestDeleteRedirect({
            redirect: redirect,
            submit: function() {
                vm.updateList();
                overlayService.close();
            }
        });
    };

    // Initial pagination options
    vm.pagination = {
        text: "",
        page: 1,
        pages: 0,
        limit: 11,
        offset: 0,
        pagination: []
    };

    // Loads the previous page
    vm.prev = function () {
        if (vm.pagination.page > 1) vm.updateList(vm.pagination.page - 1);
    };

    // Loads the next pages
    vm.next = function () {
        if (vm.pagination.page < vm.pagination.pages) vm.updateList(vm.pagination.page + 1);
    };

    // Updates the list based on current arguments
    vm.updateList = function (page) {

        vm.loading = true;

        // If a page is specified, we load that page
        page = (page ? page : vm.pagination.page);

        // Declare the arguments (making up the query string) for the call to the API
        const args = {
            limit: vm.pagination.limit,
            page: page
        };

        vm.activeFilters = 0;

        // Any filters?
        if (vm.filters.rootNode && vm.filters.rootNode.key) {
            args.rootNodeKey = vm.filters.rootNode.key;
            vm.activeFilters++;
        }

        // Any filters?
        if (vm.filters.type.value !== "all") {
            args.type = vm.filters.type.value;
            vm.activeFilters++;
        }

        if (vm.filters.text) {
            args.text = vm.filters.text;
            vm.activeFilters++;
        }

        // Declare the HTTP options
        const http = $http({
            method: "GET",
            url: `${baseUrl}GetRedirects`,
            params: args
        });

        // Show the loader for at least 200 ms
        const timer = $timeout(function () { }, 200);

        // Wait for both the AJAX call and the timeout
        $q.all([http, timer]).then(function (array) {

            vm.loading = false;
            vm.redirects = array[0].data.items;

            // Update our pagination model
            vm.pagination = array[0].data.pagination;
            vm.pagination.pagination = [];

            const from = Math.max(1, vm.pagination.page - 7);
            const to = Math.min(vm.pagination.pages, vm.pagination.page + 7);

            for (let i = from; i <= to; i++) {
                vm.pagination.pagination.push({
                    page: i,
                    active: vm.pagination.page === i
                });
            }

            const tokens = [
                vm.pagination.from,
                vm.pagination.to,
                vm.pagination.total,
                vm.pagination.page,
                vm.pagination.pages
            ];

            localizationService.localize("redirects_pagination", tokens).then(function (value) {
                vm.pagination.text = value;
            });

        }, function () {
            notificationsService.error("Unable to load redirects", "The list of redirects could not be loaded.");
            vm.loading.list = false;
        });

    };

    skybrudRedirectsService.getRootNodes().then(function (r) {
        r.data.items.forEach(function (rootNode) {
            vm.rootNodes.push(rootNode);
        });
    });

    vm.updateList();

    vm.filterUpdated = function () {
        vm.updateList();
    };

    vm.buttonGroups = [
        {
            alias: "add",
            buttonStyle: "success",
            defaultButton: {
                labelKey: "redirects_addNewRedirect",
                handler: vm.addRedirect
            },
            subButtons: []
        },
        {
            alias: "refresh",
            defaultButton: {
                labelKey: "redirects_reload",
                handler: vm.updateList
            },
            subButtons: []
        }
    ];

    eventsService.emit("skybrud.umbraco.redirects.dashboard.init", {
        buttonGroups: vm.buttonGroups,
        dashboard: {
            updateList: vm.updateList
        }
    });

});